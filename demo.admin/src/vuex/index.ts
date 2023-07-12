import { createStore } from 'vuex'
import { TagModel } from '../class/TagModel';
const store = createStore({

    //状态变量
    state() {
        return {
            Token: localStorage["token"],
            NickName: localStorage["nickname"],
            TagArrs: [] as TagModel[],//存放tag标签
            menuItems: [],//存放所有跳转路由地址的数组
            openTab: [],
            activeIndex: ''
        }

    },
    //方法
    mutations: {
        SettingNickName(state: any, NickName) {
            state.NickName = NickName;

        },
        SettingToken(state: any, Token) {
            state.Token = Token
        },
        //添加tag标签
        AddTag(state: any, tag: TagModel) {
            //判断是否已经存在
            if ((state.TagArrs as TagModel[]).filter(item => item.index.indexOf(tag.index) > -1).length == 0) {
                state.TagArrs.push(tag);
            }
        },
        //添加tab页
        addTabs(state: any, data) {
            state.openTab.push(data);
        },
        //关闭tab
        deleteTabs(state: any, route) {
            let index = 0;
            for (let gohh of state.openTab) {
                if (gohh.route === route) {
                    break;
                }
                index++;
            }
            state.openTab.splice(index, 1);
        },
        //激活tab页
        setActiveIndex(state: any, index) {
            state.activeIndex = index;
        }

    }
});
export default store;