<script lang="ts" setup>
import { ref, onMounted, toRef } from "vue";
import HeaderCom from "../../components/HeaderCom.vue";
import TreeMenuVue from "../../components/TreeMenu.vue";
import tabsCom from "../../components/tabsCom.vue";
import { Router, useRouter } from "vue-router";
import { useStore } from "vuex";
import { TagModel } from "../../class/TagModel";
import { getUserMenus } from "../../http/index";
import { MenuModel } from "../admin/menu/class/MenuModel";

const url = ref("/images/logo.ico");
url.value = "/images/logo.0606fdd2.png";
const res = ref();

const store = useStore();
const activeIndex = store.state.activeIndex;
console.log(activeIndex);
//let router: Router = useRouter();
onMounted(async () => { 
  
  res.value = (await getUserMenus()) as any as MenuModel[];
  // activeIndex.value=store.state.activeIndex;
   console.log('当前活动tab'+store.state.activeIndex);
});
//创建路由
const router = useRouter();
const handleSelect = (index: string) => {
  //根据index从路由列表中获取name
  // let name = router.getRoutes().filter((item) => item.path == "/" + index)[0]
  //   .name as string;
  // console.log("我点击的是：" + index + ",name是：" + name);
  //修改vuex的值
  // let model = new TagModel();
  // model.index = index;
  // model.name = name;
  // store.commit("AddTag", model);
  // store.state.menuItems.push({
  //   path: index,
  //   name: name,
  // });
};
</script>
<template>
  <div class="common-layout">
    <el-container>
      <el-aside class="aside-menu">
        <el-row>
          <el-col :span="24">
            <div class="homepageLogo">
              <ul>
                <li>
                  <el-image
                    style="width: 50px; height: 50px"
                    :src="url"
                    fit="fill"
                  />
                </li>
                <li><span>通用工作平台</span></li>
              </ul>
            </div>
          </el-col>
        </el-row>
        <el-row class="tac">
          <el-col :span="24">
            <el-menu
            mode="vertical"
              active-text-color="#ffd04b"
              background-color="#1F2D3D"              
              class="el-menu-vertical-demo"
              :default-active="String($route.path)"
              text-color="#ffffff"
              router                         
            >
              <!-- <el-menu-item index="/personinfo" route="/personinfo">
                <template #title>
                  <el-icon>
                    <list />
                  </el-icon>
                  <span>信息编辑</span>
                </template>
              </el-menu-item> -->
              <TreeMenuVue :list="res"></TreeMenuVue>
            </el-menu>
          </el-col>
        </el-row>
      </el-aside>
      <el-container>
        <el-header>
          <HeaderCom></HeaderCom>
        </el-header>
        <el-main>
          <tabsCom></tabsCom>
          <!-- <router-view></router-view>          -->
        </el-main>
      </el-container>
    </el-container>
  </div>
</template>
<style lang="scss" scoped>
.homepageLogo {
  height: 50px;
  line-height: 50px;

  span {
    color: white;
    font-size: 24px;
  }

  ul {
    li {
      float: left;
      margin-left: 5px;
    }
  }
}
.common-layout .el-header,
.el-container .el-header {
  height: 50px;
}
</style>
