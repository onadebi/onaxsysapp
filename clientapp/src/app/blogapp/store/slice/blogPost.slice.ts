import {createAsyncThunk, createSlice, PayloadAction } from "@reduxjs/toolkit";
import { BlogPost } from "../../models/blog_post";
import { appServices } from "../../../common/services/appservices";


export const fetchAllBlogPosts = createAsyncThunk(
    'blog/getBlogPosts',
    async () => {
        const response = await appServices.blogService.getBlogPosts();
        const allPosts : BlogPost[] = response.result ?? [];
        return allPosts;
    }
);


const initBlogPosts : BlogPost[] = [];

export const BlogPostSlice = createSlice({
    name: 'blogPostSlice',
    initialState: initBlogPosts,
    reducers: {
        setBlogPosts: (state, action) => {
            state = action.payload;
            return state;
        },
        clearBlogPosts: () => {
            return initBlogPosts;
        }
    },
    extraReducers: (builder) => {
        builder.addCase(fetchAllBlogPosts.pending, (state) => {
            appServices.commonService.LogActivity('fetching blog posts...', state);
        })
        .addCase(fetchAllBlogPosts.fulfilled, (state, action: PayloadAction<BlogPost[]>) => {
            state = action.payload;
            return state;
        })
        .addCase(fetchAllBlogPosts.rejected, (_state, action) => {
            appServices.commonService.LogError('Error fetching blog posts...', action.error);
        })
    }
});

export const { setBlogPosts, clearBlogPosts } = BlogPostSlice.actions;