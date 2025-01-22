import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import appsettings from "../../../app/common/config/appsettings";


export type CommonState = { display: boolean, message : string, icon?: string };
const initState = { display: false, message : "", icon: appsettings.public.assets.images.rocket };

export const CommonSlice = createSlice({
    name: 'common',
    initialState: initState,
    reducers: {
        hideLoader: (state) => {
            state.display = false;
            state.message = "";
        },
        setLoading: (state, action: PayloadAction<CommonState>) => {
            switch (action.payload.display) {
                case true:
                    state.display = true;
                    state.message = action.payload.message;
                    state.icon = action.payload.icon ? action.payload.icon : initState.icon;
                    break;
                case false:
                    state.display = false;
                    state.message = "";
                    state.icon = initState.icon;
                    break;
                default:
                    return state;
            }
        }
    }
});

export const { hideLoader, setLoading } = CommonSlice.actions;