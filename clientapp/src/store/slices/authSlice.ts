
import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { AuthProfile } from "../../app/common/models/AuthProfile";

const authProfileInitState: AuthProfile =  {} as AuthProfile;

export const AuthSlice = createSlice({
    name: 'auth',
    initialState: authProfileInitState,
    reducers: {
        setAuthProfile: (state, action: PayloadAction<AuthProfile>) => {
            state = action.payload;
            return state;
        },
        clearAuthProfile: () => {
            return authProfileInitState;
        }
    }
});

export const { setAuthProfile, clearAuthProfile } = AuthSlice.actions;