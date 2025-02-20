import { combineReducers } from "@reduxjs/toolkit";
import { AuthSlice } from "./slices/authSlice";
import { CommonSlice } from "./slices/common/Common.slice";
import { BlogPostSlice } from "../app/core/store/slice/blogPost.slice";

export const RootReducer = combineReducers({
    common: CommonSlice.reducer,
    auth: AuthSlice.reducer,
    // allStore,
    blogPosts: BlogPostSlice.reducer,
});
