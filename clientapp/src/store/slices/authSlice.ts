
import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { UserLoginResponse } from "../../app/common/models/UserLoginResponse";

const authProfileInitState: UserLoginResponse =  {} as UserLoginResponse;

export const AuthSlice = createSlice({
    name: 'auth',
    initialState: authProfileInitState,
    reducers: {
        setAuthProfile: (state, action: PayloadAction<UserLoginResponse>) => {
            state = action.payload;
            return state;
        },
        clearAuthProfile: () => {
            return authProfileInitState;
        },
        getAuthProfile: (state) => {
            return state;
        }
    }
});

export const { setAuthProfile, clearAuthProfile } = AuthSlice.actions;