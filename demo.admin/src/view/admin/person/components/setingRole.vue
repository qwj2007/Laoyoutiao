<template>
    <el-dialog v-model="setingRoleVisible" title="分配角色" width="50%" :before-close="handleClose">
        <div class="content">
            <el-table :data="tableData" style="width: 100%" ref="multipleTableRef">
                <el-table-column type="selection" width="55" />
                <el-table-column label="序号" width="70">
                    <template #default="scope">
                        <div>{{ scope.row.id }}</div>
                    </template>
                </el-table-column>
                <el-table-column label="名称">
                    <template #default="scope">
                        <div>{{ scope.row.name }}</div>
                    </template>
                </el-table-column>
                <el-table-column label="描述">
                    <template #default="scope">
                        <div>{{ scope.row.desc }}</div>
                    </template>
                </el-table-column>
                <el-table-column label="排序">
                    <template #default="scope">
                        <div>{{ scope.row.order }}</div>
                    </template>
                </el-table-column>
            </el-table>
            <div class="btn">
                <el-button type="primary" @click="submit()">确定</el-button>
                <el-button @click="close()">取消</el-button>
            </div>
        </div>
    </el-dialog>
</template>

<script lang="ts" setup>
import { ref, defineProps, computed, defineEmits, onMounted } from 'vue'
import { Role } from '../../role/class/Role'
import { getRoleData, settingRole } from '../../../../http/index'
import type { ElTable } from 'element-plus'
import { ElMessage } from 'element-plus'
const props = defineProps({
    setingRoleVisible: Boolean,
    personId: String
})
const emits = defineEmits(["CloseSetingRole"])
const multipleTableRef = ref<InstanceType<typeof ElTable>>() 
const handleClose = (done: () => void) => { 
    emits("CloseSetingRole")
}

//表格
const tableData = ref<Array<Role>>()
onMounted(async () => {
    let parms = {
        Name: "",
        IsEnable: true,
        Description: "",
        PageIndex: 1,
        PageSize: 10
    }
    console.log('这是加载设置角色页面。。。。。。')
    let res = await getRoleData(parms) as any
    tableData.value = res.data
    console.log(res.data)
    
})


const submit = async () => { 
    const pid = props.personId as string
    //获取当前选择的行
    let arr: any[] = multipleTableRef.value?.getSelectionRows()
    if (arr.length == 0) {
        ElMessage({
            message: '请选择需要分配的角色！',
            type: 'warning',
        }) 
    }
    else {
        const rids = arr.map((s) => { return "'" + s.id + "'" }).join(",")
        const res=await settingRole(pid, rids) as any as boolean
        if(res)
        {
            ElMessage({
                message: '设置成功！',
                type: 'success',
            }) 
        }else{
            ElMessage({
                message: '设置失败！',
                type: 'error',
            })
        }
        emits("CloseSetingRole")
    }
}
const close = () => {
    emits("CloseSetingRole")
}


</script>
<style lang="scss" scoped>
.content {
    min-height: 500px;

    .btn {
        position: absolute;
        bottom: 10px;
        left: 45%;
    }
}
</style>
