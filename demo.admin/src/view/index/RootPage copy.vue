<script lang="ts" setup>
import { ref, onMounted, toRef } from "vue";
import HeaderCom from "../../components/HeaderCom.vue";
import TreeMenuVue from "../../components/TreeMenu.vue";
import { Router, useRouter } from "vue-router";
import { useStore } from "vuex";
import { TagModel } from "../../class/TagModel";
import { getUserMenus } from "../../http/index";
import { MenuModel } from "../admin/menu/class/MenuModel";

const url = ref("/images/logo.ico");
url.value = "/images/logo.0606fdd2.png";
const res = ref();

const store = useStore();
//let router: Router = useRouter();
onMounted(async () => {    
  res.value = (await getUserMenus()) as any as MenuModel[]; 
});
//创建路由
const router = useRouter();
const handleSelect = (index: string) => {
console.log(router.getRoutes());
  //根据index从路由列表中获取name
  let name = router.getRoutes().filter((item) => item.path == "/" + index)[0].name as string;
  //修改vuex的值
  let model = new TagModel();
  model.index = index;
  model.name = name;
  store.commit("AddTag", model);
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
              active-text-color="#ffd04b"
              background-color="#545c64"
              class="el-menu-vertical-demo"
              default-active="/desktop"
              text-color="#fff"
              router
              @select="handleSelect"
            >
              <!-- 默认会有个首页的入口 -->
              <!-- <el-menu-item index="/desktop">
                <template #title>
                  <el-icon>
                    <list />
                  </el-icon>
                  <span>工作台</span>
                </template> 
              </el-menu-item>-->
              <el-menu-item index="/personinfo">
                <template #title>
                  <el-icon>
                    <list />
                  </el-icon>
                  <span>信息编辑</span>
                </template>
              </el-menu-item>
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
          <router-view></router-view>
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
</style>
