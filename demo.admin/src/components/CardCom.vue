
<script lang="ts" setup>
import { User,Comment,Money,Ship } from '@element-plus/icons-vue'
import { defineProps } from 'vue';
import { CardModel } from '../class/CardModel'
defineProps({
    info: CardModel
})
</script>
<template>
    <div class="card">
        <div class="left">
            <user v-if="info?.Icon == 'User'" />
            <Comment v-if="info?.Icon == 'Comment'" />
            <Money v-if="info?.Icon == 'Money'" />
            <Ship v-if="info?.Icon == 'Ship'" />
        </div>
        <div class="right">
            <div><span>{{ info?.Title }}</span></div>
            <div><span>{{ info?.Count }}</span></div>
        </div>
    </div>
</template>

<style lang="scss" scoped>
.card {
  height: 50px;
  line-height: 25px;
}

.left {
  width: 50px;
  height: 50px;
  float: left;

  .el-icon {
    font-size: 50px;
  }
}

.right {
  width: 100px;
  height: 50px;
  float: right;
}
</style>
