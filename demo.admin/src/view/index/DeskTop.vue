<script  lang="ts" setup type="text/javascript">
import { ref,onMounted } from "vue";

import CardCom from "../../components/CardCom.vue";
import Pie from "../../components/echarts/Pie.vue";
import Histogram from "../../components/echarts/Histogram.vue";
import Line from "../../components/echarts/Line.vue";
 import { getUserMenus } from "../../http/index";
import { MenuModel } from "../admin/menu/class/MenuModel";
const res = ref();
onMounted(async () => {
  res.value = (await getUserMenus()) as any as MenuModel[];
  console.log(res.value);
});

const list = ref([
  {
    Title: "新增用户",
    Icon: "User",
    Count: 10291,
  },
  {
    Title: "未读消息",
    Icon: "Comment",
    Count: 8912,
  },
  {
    Title: "成交金额",
    Icon: "Money",
    Count: 9280,
  },
  {
    Title: "购物总量",
    Icon: "Ship",
    Count: 13600,
  },
]);

</script>


<template>
  <div class="cardContent">
    <el-card class="box-card" v-for="item in list" :key="item.Title">
      <CardCom :info="item"></CardCom>
    </el-card>
    <el-card class="left">
      <Pie></Pie>
    </el-card>
    <el-card class="right">
      <Histogram></Histogram>
    </el-card>
    <el-card class="lineCard">
      <Line></Line>
    </el-card>
  
  </div>
</template>

<style lang="scss" scoped>
.cardContent {
  width: 100%;
  margin: 0px auto;

  .box-card {
    float: left;
    width: 24%;
    margin-right: 5px;
    margin-bottom: 20px;
  }

  .left,
  .right {
    float: left;
    width: 48%;
    margin-bottom: 20px;
  }

  .lineCard {
    width: 97.5%;
  }

  .right {
    margin-left: 20px;
  }
}
</style>