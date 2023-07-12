<template>
  <el-tabs
    v-model="editableTabsValue"
    type="card"
    :closable="editableTabs.length>1"    
    class="demo-tabs tabs"    
    tab-click="handleTabsClick"
    tab-remove="handleTabRemove"
  >
    <el-tab-pane
    class="tabs"
      v-for="item in editableTabs"
      :key="item.path"
      :label="item.name!='root'?item.name:'工作台'"
      :name="item.name"
    >     
     <router-view></router-view>
    </el-tab-pane> 
   
  </el-tabs>
</template>
<script lang="ts" setup>
import {ref,watch,onMounted} from "vue"
import {stringifyQuery, useRoute, useRouter} from 'vue-router'
import { useStore } from 'vuex'
const router=useRouter();
const route=useRoute();
const editableTabsValue = ref("/desktop");
const store = useStore();

let tabIndex = 1;
const editableTabs=store.state.menuItems;
console.log(editableTabs)
const handleTabsClick=(path:string )=>{
  console.log(path)
  console.log("点击tab"+ path);
  // router.push({
  //   path:path
  // });
}
const handleTabRemove=(name:string)=>store.commit("removeMeuItemItem",name);
watch(()=>route.path,()=>{
editableTabsValue.value=route.path
if(store.state.menuItems.every((v: { path: string; })=>v.path!==route.path)){
  store.state.menuItems.push({
    path:route.path,
    name:route.meta.title
  });
}
},{immediate:true});

// const editableTabs = ref([
//   {
//     title: "工作台",
//     name: "1",
//     content: "Tab 1 content",
//   },
// ]);
onMounted(()=>{
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
.demo-tabs > .el-tabs__content {
  padding: 32px;
  color: #6b778c;
  font-size: 16px;
  font-weight: 600;
}
</style>
