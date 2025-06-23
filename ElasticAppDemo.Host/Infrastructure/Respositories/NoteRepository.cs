using ElasticAppDemo.Host.Models;
using Elasticsearch.Net;
using Nest;
using System.Collections.Generic;

namespace ElasticAppDemo.Host.Infrastructure.Respositories
{
    public class NoteRepository : ElasticRepositoryBase<Note>, INoteRepository
    {
        public NoteRepository(IElasticProxy elasticProxy) : base(elasticProxy) { }
        protected override string IndexName => "notebook";

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public override async Task<Tuple<int, IList<Note>>> QueryAsync(int page, int limit)
        {
            var query = await this.Client.SearchAsync<Note>(x => x.Index(this.IndexName)
                .From((page - 1) * limit)
                .Size(limit)
               // .Sort(s => s.Descending(v => v.updateTime))
               );
            return new Tuple<int, IList<Note>>(Convert.ToInt32(query.Total), query.Documents.ToList());
        }

        /// <summary>
        /// 查询结果中包含指定昵称的笔记，并高亮显示
        /// </summary>
        /// <param name="key">搜索的字符串</param>
        /// <returns></returns>
        public async Task<IList<Note>> QueryByNameWithHighlightAsync(string key)
        {
            var query = await this.Client.SearchAsync<Note>(x => x.Index(this.IndexName)
            .Query(q => q.Match(m => m.Field(f => f.nickname).Query(key)))
            .Highlight(
                h => h.Fields(
                hf => hf.Field(f => f.nickname).PreTags("<strong>").PostTags("</strong>")
                )))
            ;
            //处理高亮结果
            foreach (var hit in query.Hits)
            {
                if (hit.Highlight != null && hit.Highlight.ContainsKey("nickname"))
                {
                    hit.Source.highlightTitle = string.Join(" ", hit.Highlight["nickname"]);
                }
            }
            return query.Documents.ToList();
        }
        /// <summary>
        /// bool查询，多个字段都要匹配--and查询
        /// </summary>
        /// <param name="title"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<IList<Note>> QueryByNameAsync(string title, string name)
        {
            var query = await this.Client.SearchAsync<Note>(x => x.Index(this.IndexName)
            .Query(
               q => q.Bool(b => b
                   .Must(
                       m => m.Match(mm => mm.Field(f => f.title).Query(title)),
                       m => m.Match(mm => mm.Field(f => f.nickname).Query(name))
                   )
               )
            ));
            return query.Documents.ToList();
        }
        /// <summary>
        /// 查询笔记，多个字段匹配--or查询
        /// </summary>
        /// <param name="strSearchName"></param>
        /// <returns></returns>
        public async Task<IList<Note>> QueryByTitleOrNameAsync(string strSearchName)
        {
            var result = this.Client.SearchAsync<Note>(x => x.Index(this.IndexName)
              .Query(q => q.Bool(b => b
                  .Should(
                      s => s.Match(m => m.Field(f => f.title).Query(strSearchName)),
                      s => s.Match(m => m.Field(f => f.nickname).Query(strSearchName))
                  )
              )));
            return (await result).Documents.ToList();
        }

        /// <summary>
        /// multi-match查询,查询多个字段都包含keyword,模糊查询
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public async Task<IList<Note>> QueryByMutiMatchAsync(string keyword)
        {
            var result = await this.Client.SearchAsync<Note>(x => x.Index(this.IndexName)
            .Query(q => q.MultiMatch(
                mm => mm.Query(keyword).Fields(f => f
                    .Field(ff => ff.title)
                    .Field(ff => ff.nickname)
                ))
            )
            );
            return result.Documents.ToList();
        }

        /// <summary>
        /// 自定义mapping创建索引
        /// </summary>
        /// <returns></returns>
        public async Task CreateIndexWithCustomMappingAsync()
        {
            var existsResponse = await this.Client.Indices.ExistsAsync(IndexName);
            //如果索引不存在，则创建索引
            if (!existsResponse.Exists)
            {

                var createIndexResponse = await this.Client.Indices.CreateAsync(IndexName, c => c
                    .Map<Note>(m => m
                        .Properties(p => p
                        .Keyword(k => k.Name(n => n.Id))
                        .Keyword(k => k.Name(n => n.noteId))
                        .Keyword(k => k.Name(n => n.cover))
                        .Keyword(k => k.Name(n => n.avatar))
                        .Keyword(k => k.Name(n => n.highlightTitle))
                        .Text(t => t.Name(n => n.title).Analyzer("ik_max_word"))
                        .Text(t => t.Name(n => n.nickname).Analyzer("ik_max_word"))
                        .Number(n => n.Name(n => n.likeTotal).Type(NumberType.Integer))
                        .Number(n => n.Name(n => n.commentTotal).Type(NumberType.Integer))
                        .Number(n => n.Name(n => n.collectTotal).Type(NumberType.Integer))
                        .Date(d => d.Name(n => n.updateTime))
                        )
                    )
                );
                if (!createIndexResponse.IsValid)
                {
                    throw new Exception($"Failed to create index: {createIndexResponse.ServerError.Error.Reason}");
                }
                if (!createIndexResponse.Acknowledged)
                {
                    throw new Exception($"Failed to create index: {createIndexResponse.ServerError.Error.Reason}");
                }

            }

        }

        /// <summary>
        /// 自动映射属性创建索引
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task CreateIndexAsync()
        {
            // 创建索引
            var createIndexResponse = await this.Client.Indices.CreateAsync(IndexName, c => c
                .Map<Note>(m => m.AutoMap()//自动映射属性                   

                )
            );
            if (!createIndexResponse.IsValid)
            {
                throw new Exception($"Failed to create index: {createIndexResponse.ServerError.Error.Reason}");
            }
        }

        /// <summary>
        /// function_score自定义分数查询
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public async Task<IList<Note>> QueryWithFunctionScoreAsync(string keyword)
        {
            var result = await this.Client.SearchAsync<Note>(x => x.Index(this.IndexName)
            .Query(
                q => q.FunctionScore(fs => fs.Query(
                    q => q.MultiMatch(mm => mm.Query(keyword).Fields(f => f
                                                     .Field(ff => ff.title, 2.0) //设置权重值为2.0
                                                     .Field(ff => ff.nickname)//不设置默认为1.0
                                                 )))


                .Functions(
                    fns => fns.FieldValueFactor(fvf => fvf.Field(f => f.commentTotal) //commentTotal字段作为评分依据

                                                                 .Factor(0.2) // 设置因子为0.5
                                                                 .Missing(0) // 如果字段缺失，则使用0
                                                                 .Modifier(FieldValueFactorModifier.SquareRoot)))// 平方根修正因子



                .Functions(fns => fns.FieldValueFactor(fvf => fvf.Field(f => f.likeTotal) //likeTotal字段作为评分依据
                                                                 .Factor(0.5) // 设置因子为0.5
                                                                 .Missing(0) // 如果字段缺失，则使用0
                                                                 .Modifier(FieldValueFactorModifier.SquareRoot)))// 平方根修正因子)
                .Functions(fns => fns.FieldValueFactor(fvf => fvf.Field(f => f.collectTotal)//collectTotal字段作为评分依据
                                                                  .Factor(0.3) // 设置因子为0.3
                                                                  .Missing(0)
                                                                  .Modifier(FieldValueFactorModifier.SquareRoot)))// 平方根修正因子)
                .ScoreMode(FunctionScoreMode.Sum)// 设置评分模式为Sum
                .BoostMode(FunctionBoostMode.Sum) // 设置提升模式为Sum
                ))

            .Sort(s => s.Descending(SortSpecialField.Score))
            .From(0)
            .Size(100)
            .Highlight(
                h => h.Fields(
                hf => hf.Field(f => f.nickname).PreTags("<strong>").PostTags("</strong>")
                ))
            );
            //处理高亮结果
            foreach (var hit in result.Hits)
            {
                if (hit.Highlight != null && hit.Highlight.ContainsKey(nameof(Note.nickname)))
                {
                    hit.Source.highlightTitle = string.Join(" ", hit.Highlight[nameof(Note.nickname)]);
                }
            }
            return result.Documents.ToList();
        }

        /// <summary>
        /// 按发布者昵称分组统计笔记数量，并统计点赞、评论、收藏的总和
        /// </summary>
        /// <returns></returns>
        public async Task<IList<NoteAgg>> GroupByNicknameAggAsync()
        {
            var result = await this.Client.SearchAsync<Note>(s => s
                .Index(this.IndexName)
                .Size(100) // 不返回文档，只返回聚合结果
                .Aggregations(agg => agg
                    .Terms("nickname_group", t => t
                        .Field(f => f.noteId) //在 Elasticsearch 里，字段的类型会对它的使用方式产生限制。以nickname字段为例：
                                              //若它是text类型，那么在进行聚合操作时，默认会采用分词后的词条，这就可能导致聚合结果出现偏差。
                                              //若它是keyword类型，就能够直接用于聚合操作。
                        .Size(100)
                        .Aggregations(sub => sub
                            .Sum("like_total_sum", sm => sm.Field(f => f.likeTotal))
                            .Sum("comment_total_sum", sm => sm.Field(f => f.commentTotal))
                            .Sum("collect_total_sum", sm => sm.Field(f => f.collectTotal))
                        )
                    )
                )
            );
            var noteAggs = new List<NoteAgg>();
            var buckets = result.Aggregations.Terms("nickname_group").Buckets;
            if (buckets != null)
            {
                foreach (var bucket in buckets)
                {
                    var agg = new NoteAgg
                    {
                        //countVal = (int)(bucket.ValueCount("note_count")?.Value ?? 0),
                        likeTotal = (int)(bucket.Sum("like_total_sum")?.Value ?? 0),
                        commentTotal = (int)(bucket.Sum("comment_total_sum")?.Value ?? 0),
                        collectTotal = (int)(bucket.Sum("collect_total_sum")?.Value ?? 0)
                    };
                    noteAggs.Add(agg);
                }

            }

            return noteAggs;
        }

        /// <summary>
        /// 按 noteId 分组统计笔记数量，并统计点赞、评论、收藏的总和
        /// </summary>
        /// <returns></returns>
        public async Task<IList<NoteAgg>> GroupByNoteIdAggAsync()
        {
            var result = await this.Client.SearchAsync<Note>(s => s
                .Index(this.IndexName)
                .Size(100) // 只返回聚合结果，不返回文档
                .Aggregations(agg => agg
                    .Terms("noteid_group", t => t
                        .Field(f => f.noteId)
                        .Size(1000)
                        .Aggregations(sub => sub
                            .ValueCount("note_count", vc => vc.Field(f => f.noteId))
                            .Sum("like_total_sum", sm => sm.Field(f => f.likeTotal))
                            .Sum("comment_total_sum", sm => sm.Field(f => f.commentTotal))
                            .Sum("collect_total_sum", sm => sm.Field(f => f.collectTotal))
                        )
                    )
                )
            );
            var noteAggs = new List<NoteAgg>();
            var buckets = result.Aggregations.Terms("noteid_group").Buckets;
            if (buckets != null)
            {
                foreach (var bucket in buckets)
                {
                    var agg = new NoteAgg
                    {
                        countVal = (int)(bucket.ValueCount("note_count")?.Value ?? 0),
                        likeTotal = (int)(bucket.Sum("like_total_sum")?.Value ?? 0),
                        commentTotal = (int)(bucket.Sum("comment_total_sum")?.Value ?? 0),
                        collectTotal = (int)(bucket.Sum("collect_total_sum")?.Value ?? 0)
                    };
                    noteAggs.Add(agg);
                }

            }

            return noteAggs;
        }

        /// <summary>
        /// 按 noteId 分组统计笔记数量，并统计点赞、评论、收藏的总和
        /// </summary>
        /// <returns></returns>
        public async Task<IList<NoteAgg>> GroupByNoteAggAsync()
        {

            var result = await this.Client.SearchAsync<Note>(s => s
                .Index(this.IndexName)
                .Size(100) // 只返回聚合结果，不返回文档
                .Aggregations(agg => agg
                    .Terms("noteid_group", t => t
                        .Field(f => f.noteId)
                        .Size(1000)
                        .Aggregations(sub => sub
                            .ValueCount("note_count", vc => vc.Field(f => f.noteId))
                            .Sum("like_total_sum", sm => sm.Field(f => f.likeTotal))
                            .Sum("comment_total_sum", sm => sm.Field(f => f.commentTotal))
                            .Sum("collect_total_sum", sm => sm.Field(f => f.collectTotal))
                        )
                    )
                )
            );
            var noteAggs = new List<NoteAgg>();
            var buckets = result.Aggregations.Terms("noteid_group").Buckets;
            if (buckets != null)
            {
                foreach (var bucket in buckets)
                {
                    var agg = new NoteAgg
                    {
                        countVal = (int)(bucket.ValueCount("note_count")?.Value ?? 0),
                        likeTotal = (int)(bucket.Sum("like_total_sum")?.Value ?? 0),
                        commentTotal = (int)(bucket.Sum("comment_total_sum")?.Value ?? 0),
                        collectTotal = (int)(bucket.Sum("collect_total_sum")?.Value ?? 0)
                    };
                    //this.Client.IndexDocument<NoteAgg>(agg);
                    noteAggs.Add(agg);
                }

            }

            return noteAggs;
        }

        /// <summary>
        /// 批量插入文档
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> InsertManyDocument()
        {
            var list = new List<Note>();
            for (int i = 1000; i < 2000; i++)
            {
                Note note = new Note
                {
                    Id = "10000" + i,
                    noteId = 10000 + i,
                    title = $"笔记标题{i}",
                    nickname = $"用户昵称{i}",
                    cover = "https://example.com/cover.jpg",
                    avatar = "https://example.com/avatar.jpg",
                    likeTotal = (i % 100).ToString(),
                    commentTotal = (i % 50).ToString(),
                    collectTotal = (i % 30).ToString(),
                    updateTime = DateTime.Now.Microsecond.ToString()
                };
                list.Add(note);
            }
            var batchSize = 200;// 每批次插入200条数据
            for (int i = 0; i < list.Count; i += batchSize)
            {
                var batch = list.Skip(i).Take(batchSize).ToList();
                if (!await this.IndexManyInsert(batch))
                {
                    return false; // 如果批量插入失败，返回false
                }

            }
            return true; // 如果所有批次都成功插入，返回true
            //return await this.IndexManyInsert(list);
        }

        public async Task<bool> BulkInsert()
        {
            var list = new List<Note>();
            for (int i = 2000; i < 3000; i++)
            {
                Note note = new Note
                {
                    Id = "20000" + i,
                    noteId = 20000 + i,
                    title = $"笔记标题BulkInsert{i}",
                    nickname = $"用户昵称BulkInsert{i}",
                    cover = "https://example.com/cover.jpg",
                    avatar = "https://example.com/avatar.jpg",
                    likeTotal = (i % 100).ToString(),
                    commentTotal = (i % 50).ToString(),
                    collectTotal = (i % 30).ToString(),
                    updateTime = DateTime.Now.Microsecond.ToString()
                };
                list.Add(note);
            }
            var batchSize = 200;// 每批次插入200条数据
            for (int i = 0; i < list.Count; i += batchSize)
            {
                var batch = list.Skip(i).Take(batchSize).ToList();
                if (!await this.IndexManyInsert(batch))
                {
                    return false; // 如果批量插入失败，返回false
                }

            }
            return true; // 
        }

        /// <summary>
        /// 使用 Scroll API 实现深度分页（适合大数据量场景，游标方式）大数据量导出
        /// 基于快照的批量分页
        /// 原理：通过 scroll 参数生成一个“搜索上下文快照”（类似数据库的游标），
        /// 后续请求通过 scroll_id 从快照中获取数据。
        /// 快照在集群内存中临时保存（通过 scroll=1m 指定超时时间），避免重复计算。
        /// </summary>
        /// <param name="pageSize">每页条数</param>
        /// <param name="scrollTime">scroll上下文有效期，如"2m"</param>
        /// <returns>所有Note文档</returns>
        public async Task<IList<Note>> ScrollSearchAsync(int pageSize = 100, string scrollTime = "2m")
        {
            var allNotes = new List<Note>();
            // 第一次请求，获取scrollId
            var searchResponse = await this.Client.SearchAsync<Note>(s => s
                .Index(this.IndexName)
                .From(0)
                .Size(pageSize)
                .Scroll(scrollTime)
                .Sort(ss => ss.Descending(f => f.Id))
                .Query(q => q.MatchAll(
                 //按条件查询
                 ))
            );
            if (!searchResponse.IsValid)
            {
                Console.WriteLine($"Scroll初始化失败: {searchResponse.DebugInformation}");
                return null;
            }
            var scrollId = searchResponse.ScrollId;
            allNotes.AddRange(searchResponse.Documents);

            // 循环获取后续数据，直到没有更多
            while (searchResponse.Documents.Count > 0)
            {
                searchResponse = await this.Client.ScrollAsync<Note>(scrollTime, scrollId);
                if (searchResponse.Documents.Count == 0)
                    break;
                allNotes.AddRange(searchResponse.Documents);
                scrollId = searchResponse.ScrollId;
            }

            // 清理scroll上下文
            await this.Client.ClearScrollAsync(new ClearScrollRequest(scrollId));

            return allNotes;
        }



        /// <summary>
        /// 使用 Search After 实现实时深度分页
        /// 仅支持“下一页”操作，无法跳转到任意页（需结合业务层缓存游标）；
        /// </summary>
        /// <param name="pageSize">每页大小（建议 50-200）</param>
        /// <param name="processPage">分页回调（处理当前页数据）</param>
        /// <param name="initialSort">初始排序规则（必须包含唯一字段组合）</param>
        public async Task PaginateWithSearchAfterAsync(
            int pageSize,
            Action<List<Note>> processPage,
            Func<SortDescriptor<Note>, IPromise<IList<ISort>>> initialSort = null)
        {
            initialSort ??= sd => sd
                 .Descending(d => d.noteId)  // 主排序：时间戳降序（最新数据在前）
                .Descending(d => d.Id);         // 次排序：ID升序（确保唯一性）

            try
            {
                // 第一页：无 search_after 参数
                var searchResponse = await Client.SearchAsync<Note>(s => s
                    .Size(pageSize)
                    .Sort(initialSort)
                    .Query(q => q.MatchAll(

                        )) // 替换为实际查询条件
                );

                if (!searchResponse.IsValid)
                {
                    Console.WriteLine($"首屏查询失败: {searchResponse.DebugInformation}");
                    return;
                }

                // 处理首屏数据
                var currentPage = 1;
                var hits = searchResponse.Hits;
                if (hits.Any())
                {
                    processPage?.Invoke(hits.Select(h => h.Source).ToList());
                    Console.WriteLine($"第 {currentPage} 页获取 {hits.Count} 条数据");
                }

                // 迭代后续页面
                while (hits.Any())
                {
                    // 获取最后一条记录的排序值（关键！）
                    var lastHit = hits.Last();
                    var sortValues = lastHit.Sorts;

                    // 使用 search_after 查询下一页
                    searchResponse = await this.Client.SearchAsync<Note>(s => s
                        .Size(pageSize)
                        .Sort(initialSort)       // 必须与首屏排序规则一致
                        .SearchAfter(sortValues) // 关键参数：基于上一页末尾排序值
                        .Query(q => q.MatchAll()) // 与首屏查询条件一致
                    );

                    if (!searchResponse.IsValid)
                    {
                        Console.WriteLine($"第 {currentPage + 1} 页查询失败: {searchResponse.DebugInformation}");
                        break;
                    }

                    hits = searchResponse.Hits;
                    currentPage++;

                    if (hits.Any())
                    {
                        processPage?.Invoke(hits.Select(h => h.Source).ToList());
                        Console.WriteLine($"第 {currentPage} 页获取 {hits.Count} 条数据");
                    }
                }

                Console.WriteLine("分页完成，无更多数据");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"分页过程中发生异常: {ex.Message}");
            }
        }

        /// <summary>
        /// // 第一页查询（无 search_after）
        /// </summary>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>

        public async Task<PagedResult<Note>> GetFirstPageAsync(int pageSize = 10)
        {
            var searchResponse = await this.Client.SearchAsync<Note>(s => s
                .Query(q => q.MatchAll())
                .Sort(ss => ss
                    .Descending(f => f.noteId)
                    .Ascending("_id")
                )
                .Size(pageSize)
            );

            if (!searchResponse.IsValid)
                throw new Exception($"查询失败: {searchResponse.ServerError.Error.Reason}");

            return new PagedResult<Note>
            {
                Items = searchResponse.Documents,
                HasMore = searchResponse.Hits.Count >= pageSize,
                ScrollId = GetScrollId(searchResponse.Hits)
            };
        }

        /// <summary>
        /// 用search_after获取下一页数据,scrollId为null时获取第一页数据,
        /// 只能获取下一页数据，不能跳转到任意页
        /// </summary>
        /// <param name="scrollId">要查询下一页的标识内容</param>
        /// <param name="pageSize">页数</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<PagedResult<Note>> GetPageBySearchAfterAsync(string scrollId=null, int pageSize = 10)
        {
           
            //if (string.IsNullOrEmpty(scrollId))
            //    return new PagedResult<Note> { Items = new List<Note>() };

            var searchAfterValues = ParseScrollId(scrollId);

            var searchResponse = await Client.SearchAsync<Note>(s => s
                .Query(q => q.MatchAll())
                .Sort(ss => ss
                    .Descending(f => f.noteId)
                    .Descending(f=>f.Id)
                )
                .SearchAfter(searchAfterValues)
                .Size(pageSize)
            );
            

            if (!searchResponse.IsValid)
                throw new Exception($"查询失败: {searchResponse.ServerError.Error.Reason}");

            return new PagedResult<Note>
            {
                Items = searchResponse.Documents,
                HasMore = searchResponse.Hits.Count >= pageSize,
                ScrollId = GetScrollId(searchResponse.Hits)
            };
        }
        /// <summary>
        /// 从最后一条记录提取 scrollId
        /// </summary>
        /// <param name="hits"></param>
        /// <returns></returns>
        // 
        private string GetScrollId(IReadOnlyCollection<IHit<Note>> hits)
        {
            if (hits.Count == 0) return null;
            var lastHit = hits.Last();
            var sort = lastHit.Sorts.ToArray();
            return $"{sort[0]}|{sort[1]}"; // 序列化排序值
        }
        /// <summary>
        /// 解析 scrollId 为 search_after 值
        /// </summary>
        /// <param name="scrollId"></param>
        /// <returns></returns>
        // 
        private object[] ParseScrollId(string scrollId)
        {
            if (string.IsNullOrEmpty(scrollId)) return null;
            var parts = scrollId.Split('|');
            return new object[]
            {
            parts[0],
            parts[1]
            };
        }

       
    }
}

