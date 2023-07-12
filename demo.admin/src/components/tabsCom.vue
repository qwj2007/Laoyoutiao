<template>
  <el-tabs
    v-model="editableTabsValue"
    type="card"
    :closable="editableTabs.length > 1"
    class="demo-tabs tabs"
    @tab-click="handleTabsClick"
    @tab-remove="handleTabRemove"
  >
    <el-tab-pane
      class="tabs"
      v-for="item in editableTabs"
      :key="item.path"
      :label="item.name != 'root' ? item.name : '工作台'"
      :name="item.path"
    >
    </el-tab-pane>
    <router-view></router-view>
  </el-tabs>
</template>
<script lang="ts" setup>
import { ref, watch, onMounted } from "vue";
import { useRoute, useRouter } from "vue-router";
import { useStore } from "vuex";
import { MenuModel } from "../view/admin/menu/class/MenuModel";

const router = useRouter();
const route = useRoute();
const editableTabsValue = ref("/desktop");
const store = useStore();
const editableTabs = store.state.menuItems;

const handleTabsClick = (tab: any) => {
  console.log("Tab单击"+tab.props.name);
  const menutoroute = JSON.parse(localStorage["menus"]) as any as MenuModel[];
  let cName:string="";
  console.log(menutoroute); 
  for(let i=0;i<menutoroute.length;i++){    
    if(tab.props.name=='/'+menutoroute[i].index){
      cName=menutoroute[i].name;
      break;
    }
  }
  router.push({
    path:tab.props.name ,
    name:cName ,
  });  
 
};
const handleTabRemove = (tabIndex: string) => {
  console.log('---------关闭----------');
 console.log(tabIndex);

  for (let i = 0; i < store.state.menuItems.length; i++) {
    if (
      tabIndex === store.state.menuItems[i].path &&
      store.state.menuItems.length > 1
    ) {
      store.state.menuItems.splice(i, 1);
      router.push({path: store.state.menuItems[i - 1].path? store.state.menuItems[i - 1].path: store.state.menuItems[i].path,
        name: store.state.menuItems[i - 1].name? store.state.menuItems[i - 1].name: store.state.menuItems[i].name,
      });
     // editableTabsValue.value = store.state.menuItems[i - 1].path? store.state.menuItems[i - 1].path: store.state.menuItems[i].path
  
    }
  }
};

//监控路由信息
watch(
  () => route.path,
  () => {
    if(route.path=='/login'){
      return;
    }
    editableTabsValue.value = route.path;
    
    if (
      store.state.menuItems.every(
        (v: { path: string }) => v.path !== route.path
      )
    ) {
      store.state.menuItems.push({
        path: route.path,
        name: route.name,
      });
      console.log("-------------------------------------");
      console.log("当前的tab页;" + editableTabsValue.value);
      store.commit("setActiveIndex",route.path);      
    }
  },
  { immediate: true }
);

// const editableTabs = ref([
//   {
//     title: "工作台",
//     name: "1",
//     content: "Tab 1 content",
//   },
// ]);
onMounted(() => {
  //handleTabsEdit("工作台","add")
});

// const removeTab = (targetName: string) => {
//   const tabs = editableTabs.value;
//   let activeName = editableTabsValue.value;
//   if (activeName === targetName) {
//     tabs.forEach((tab, index) => {
//       if (tab.name === targetName) {
//         const nextTab = tabs[index + 1] || tabs[index - 1];
//         if (nextTab) {
//           activeName = nextTab.name;
//         }
//       }
//     });
//   }
// };

// const handleTabsEdit = (targetName: string, action: "remove" | "add") => {
//   if (action === "add") {
//     const newTabName = `${++tabIndex}`;
//     editableTabs.push({
//       title: "New Tab",
//       name: newTabName,
//       content: "New Tab content",
//     });
//     editableTabsValue.value = newTabName;
//   } else if (action === "remove") {
//     const tabs = editableTabs.value;
//     let activeName = editableTabsValue.value;
//     if (activeName === targetName) {
//       tabs.forEach((tab, index) => {
//         if (tab.name === targetName) {
//           const nextTab = tabs[index + 1] || tabs[index - 1];
//           if (nextTab) {
//             activeName = nextTab.name;
//           }
//         }
//       });
//     }

//     editableTabsValue.value = activeName;
//     editableTabs.value = tabs.filter((tab) => tab.name !== targetName);
//   }
// };
</script>
<style>
.demo-tabs {
  padding: 32px;
  color: red;
  font-size: 16px;
  /* font-weight: 600; */
}
</style>
