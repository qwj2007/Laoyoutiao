<script lang="ts" setup>
import { List, Menu as IconMenu } from "@element-plus/icons-vue";
import { defineProps } from "vue";
import { TreeModel } from "../class/TreeModel";
const props = defineProps({
  list: Array as () => Array<TreeModel>,
});
</script>
<template>
  <!--单级-->
  <!-- item -->
  <el-menu-item
    v-for="item in list?.filter((s) => s.children == null)"
    :index="item.index"
    :key="item.index"    
  >
    <template #title>
      <el-icon>
        <list />
      </el-icon>
      <span>{{ item.name }}</span>
    </template>
  </el-menu-item>
  <!-- sub-item -->
  <!--多级菜单-->
  <el-sub-menu
    v-for="item in list?.filter((s) => s.children != null)"
    :index="item.index"
    :key="item.index"   
  >
    <template #title>
      <el-icon>
        <icon-menu />
      </el-icon>
      <span>{{ item.name }}</span>
    </template>
    <TreeMenu :list="item.children"></TreeMenu>
  </el-sub-menu>
</template>