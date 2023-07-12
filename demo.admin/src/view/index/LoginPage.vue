<template>
  <div class="login">
    <div class="relative">
      <div class="left">
        <el-row>
          <el-col :span="24">
            <div class="homepageLogo">
              <ul>
                <li>
                  <el-image
                    style="width: 50px; height: 50px"
                    :src="url"
                    fit="fit"
                  />
                </li>
                <li><span>DemoAdmin</span></li>
              </ul>
            </div>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="24">
            <el-image class="boxbg" :src="boxbg" fit="fit" />
            <p class="p1">欢迎使用本系统</p>
            <p class="p2">开箱即用的中后台管理系统</p>
          </el-col>
        </el-row>
      </div>
      <div class="right">
        <el-row>
          <el-col :span="24">
            <h2>登录</h2>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="24">
            <el-form
              :model="form"
              label-width="120px"
              label-position="top"
              size="large"
              class="form"
              :rules="rules"
              ref="ruleFormRef"
            >
              <el-form-item label="用户名" prop="userName">
                <el-input v-model="form.userName" />
              </el-form-item>
              <el-form-item label="密码" prop="passWord">
                <el-input
                  v-model="form.passWord"
                  type="password"
                  show-password
                />
              </el-form-item>
              <el-form-item>
                <el-button
                  class="submitBtn"
                  type="primary"
                  @click="onSubmit(ruleFormRef)"
                  >登录
                </el-button>
              </el-form-item>
            </el-form>
          </el-col>
        </el-row>
      </div>
    </div>
  </div>
</template>

<script lang="ts"  setup>
import { ref, reactive } from "vue";
import type { FormInstance, FormRules } from "element-plus";
import { ElMessage } from "element-plus";
import { getToken } from "../../http/index";
import { UserInfo } from "./class/UserInfo";
import Tool from "../../global";
import { useStore } from "vuex";
import { useRouter } from "vue-router";
import { getUserMenus } from "../../http/index";
import { MenuModel } from "../admin/menu/class/MenuModel";
const url = ref("/images/logo.0606fdd2.png");
const boxbg = ref("/images/svgs/login-box-bg.svg");

const form = reactive({ userName: "", passWord: "" });
const store = useStore();
const router = useRouter();
const res=ref();
//验证规则
const rules = reactive<FormRules>({
  userName: [{ required: true, message: "请输入用户名", trigger: "blur" }],
  passWord: [{ required: true, message: "请输入密码", trigger: "blur" }],
});
//表单实例
const ruleFormRef = ref<FormInstance>();
const onSubmit = async (ruleFormRef: FormInstance | undefined) => {
  if (!ruleFormRef) return;
  ruleFormRef.validate(async (valid, fields) => {
    if (valid) {
      const token = (await getToken(
        form.userName,
        form.passWord
      )) as any as string;
      console.log("token开始解析token");
      //用户登录信息
      const user: UserInfo = JSON.parse(new Tool().FormatToken(token));
      localStorage["token"] = token;
      localStorage["nickname"] = user.NickName;
      store.commit("SettingNickName", user.NickName);
      store.commit("SettingToken", token);

      res.value = (await getUserMenus()) as any as MenuModel[]; 
     localStorage.setItem('menus', JSON.stringify(res.value));

      router.push({ path: "/desktop" });      
    } else {
      console.log("校验不通过");
      let errors: string = "";

      fields?.userName?.forEach((element) => {
        errors += element.message + ";";
      });

      fields?.passWord?.forEach((element) => {
        errors += element.message + ";";
      });

      ElMessage({
        type: "warning",
        message: errors,
      });
    }
  });
};
</script>
<style scoped lang="scss">
.login {
  width: 100%;
  height: 100%;

  .relative {
    width: 100%;
    height: 100%;
    text-align: center;

    .left {
      width: 50%;
      height: 100%;
      float: left;
      background-image: url("/images/svgs/login-bg.svg");

      .boxbg {
        width: 350px;
        height: 350px;
        margin-top: 100px;
      }

      .homepageLogo {
        height: 50px;
        line-height: 50px;
        margin-top: 40px;

        span {
          color: white;
          font-size: 24px;
        }

        ul {
          list-style: none;

          li {
            float: left;
            margin-left: 5px;
          }
        }
      }

      p {
        color: white;
      }

      .p1 {
        font-size: 1.875rem;
        line-height: 2.25rem;
      }

      .p2 {
        font-size: 0.875rem;
        line-height: 1.25rem;
      }
    }

    .right {
      width: 50%;
      float: left;
      padding-top: 15%;

      .form {
        width: 50%;
        margin: 0px auto;

        .submitBtn {
          width: 100%;
        }
      }
    }
  }
}
</style>