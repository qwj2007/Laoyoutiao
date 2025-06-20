using ElasticAppDemo.Host.Models;
using Elasticsearch.Net;
using Nest;

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
                    q => q.MultiMatch( mm => mm.Query(keyword).Fields(f => f
                                                     .Field(ff => ff.title,2.0) //设置权重值为2.0
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
            
            .Sort(s=>s.Descending(SortSpecialField.Score))
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
                if (hit.Highlight != null && hit.Highlight.ContainsKey("nickname"))
                {
                    hit.Source.highlightTitle = string.Join(" ", hit.Highlight["nickname"]);
                }
            }
            return result.Documents.ToList();
        }
    }

}
