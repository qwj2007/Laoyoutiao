<script lang="ts" setup>
// 引入 echarts 核心模块，核心模块提供了 echarts 使用必须要的接口。
import * as echarts from "echarts";
//页面加载完成后，加载图表
//onMounted就是在vue中加载的意思
import { onMounted } from "vue";
onMounted(async () => {
  var chartDom = document.getElementById("echarMain-Pie");
  var myChart = echarts.init(chartDom as HTMLElement);
  myChart.resize({
    width: 800,
    height: 400,
  });

  //设置option
 var  option = {
    title : {
        text: '某站点用户访问来源',
        subtext: '纯属虚构',
        x:'center'
    },
    tooltip : {
        trigger: 'item',
        formatter: "{a} <br/>{b} : {c} ({d}%)"
    },
    legend: {
        orient : 'vertical',
        x : 'left',
        data:['直接访问','邮件营销','联盟广告','视频广告','搜索引擎']
    },
    toolbox: {
        show : true,
        feature : {
            mark : {show: true},
            dataView : {show: true, readOnly: false},
            magicType : {
                show: true, 
                type: ['pie', 'funnel'],
                option: {
                    funnel: {
                        x: '25%',
                        width: '50%',
                        funnelAlign: 'left',
                        max: 1548
                    }
                }
            },
            restore : {show: true},
            saveAsImage : {show: true}
        }
    },
    calculable : true,
    series : [
        {
            name:'访问来源',
            type:'pie',
            radius : '55%',
            center: ['50%', '60%'],
            data:[
                {value:335, name:'直接访问'},
                {value:310, name:'邮件营销'},
                {value:234, name:'联盟广告'},
                {value:135, name:'视频广告'},
                {value:1548, name:'搜索引擎'}
            ]
        }
    ]
};
/*
  const option = {
    title: {
      text: "访问方式",
      subtext: "数据访问",
      left: "center",
    },
    tooltip: {
      trigger: "item",
    },
    legend: {
      orient: "vertical",
      left: "left",
    },
    series: [
      {
        type: "pie",
        name: "access from",
        radius: "58%",
        data: [
          {
            value: 335,
            name: "直接访问",
          },
          {
            value: 234,
            name: "联盟广告",
          },
          {
            value: 1548,
            name: "搜索引擎",
          },
        ],
        emphasis: {
          itemStyle: {
            shadowBlur: 10,
            shadowOffsetX: 0,
            shadowColor: "rgba(0, 0, 0, 0.5)",
          },
        },
      },
    ],
  };*/
  myChart.setOption(option);
});
</script>
<template>
  <div id="echarMain-Pie"></div>
</template>