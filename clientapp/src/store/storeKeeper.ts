import { configureStore } from "@reduxjs/toolkit";
import { RootReducer } from "./RootReducer";
import appsettings from "../app/common/config/appsettings";

const storeKeeper = configureStore({
    reducer: RootReducer,
    devTools: appsettings.env.includes('dev'),
});

export type RootState = ReturnType<typeof storeKeeper.getState>;
export type AppDispatch = typeof storeKeeper.dispatch;

export default storeKeeper;