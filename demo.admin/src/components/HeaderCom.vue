<template>
  <el-row>
    <el-col :span="12">
      <el-breadcrumb  separator=">">
        <el-breadcrumb-item 
        :to="{path:item.path}"
        v-for="item in store.state.menuItems" :key="item.path"
          >{{item.name}}
          <!-- <a href="/">
            <el-icon> <house /> </el-icon><span>首页</span>
          </a> -->
          </el-breadcrumb-item>
        <!-- <el-breadcrumb-item><span>桌面</span></el-breadcrumb-item> -->
      </el-breadcrumb>
    </el-col>

    <el-col :span="12">
      <div class="dropdown">
        <el-avatar :size="30" :src="circleUrl" />
        <el-dropdown>
          <span class="el-dropdown-link">
            <span>{{ NickName }}</span>
            <el-icon class="el-icon--right">
              <arrow-down />
            </el-icon>
          </span>
          <template #dropdown>
            <el-dropdown-menu>
              <el-dropdown-item>
                <el-link :underline="false"
                  ><span @click="goToPerson"> 个人主页</span>
                </el-link>
              </el-dropdown-item>
              <el-dropdown-item>
                <span @click="logOut">退出</span>
              </el-dropdown-item>
            </el-dropdown-menu>
          </template>
        </el-dropdown>
      </div>
    </el-col>
  </el-row>
  <!-- <el-row>
    <el-divider />
    <el-col :span="24">
      <TagComVue></TagComVue>
    </el-col>
  </el-row> -->
</template>
<script lang="ts" setup>
import { ref, onMounted } from "vue";
import TagComVue from "./TagCom.vue";
import { useStore } from "vuex";
import { useRouter } from "vue-router";
import Tool from "../global";
//import Tool from '../global';
const circleUrl = ref("/images/Person.jpg");

const NickName = ref();
const router = useRouter();
const store=useStore();
onMounted(() => { 
    NickName.value = useStore().state.NickName;  
});
const goToPerson = () => {
  router.push({ path: "/personinfo" });
};
const logOut = () => {
  new Tool().ClearLocalStorage();
  store.state.menuItems=[];
  router.push({ path: "/login" });
};

const tags = ref([
  { name: "Tag 1", type: "" },
  { name: "Tag 2", type: "success" },
  { name: "Tag 3", type: "info" },
  { name: "Tag 4", type: "warning" },
  { name: "Tag 5", type: "danger" },
]);
</script>
<style lang="scss" scoped>
.el-header {
  .el-col {
    height: 30px;
    line-height: 30px;

    .el-breadcrumb {
      line-height: inherit;
    }

    .el-icon {
      margin-right: 5px;
    }

    .el-divider {
      margin: 0;
    }
  }
}
.el-divider--horizontal {
  display: block;
  height: 1px;
  width: 100%;
  margin: 0;
  border-top: 1px var(--el-border-color) var(--el-border-style);
}
</style>