
    <template>
  <el-row>
    <el-col :span="6">
      <el-form :model="form" label-width="120px">
        <el-form-item label="昵称">
          <el-input v-model="form.name" />
        </el-form-item>
        <el-form-item label="密码">
          <el-input v-model="form.password" />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="onSubmit">修改</el-button>
          <el-button>取消</el-button>
        </el-form-item>
      </el-form>
    </el-col>
  </el-row>
</template>

<script lang="ts" setup>
import { reactive } from "vue";
import { useStore } from "vuex";
const form = reactive({
  name: useStore().state.NickName,
  password: "",
});
const onSubmit = async () => {
  console.log("修改中");
};
</script>