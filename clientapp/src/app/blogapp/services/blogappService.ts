import agent from "../../common/api/agent";
import apiRoutes from "../../common/api/apiRoutes";
import appsettings from "../../common/config/appsettings";
import GenResponse, { StatusCode } from "../../common/config/GenResponse";
import { appServices } from "../../common/services/appservices";
import { BlogCategory } from "../models/blog_categories";
import { BlogPost } from "../models/blog_post";
import { BlogpostNewmodel } from "../models/BlogPostNewModel";

export default class BlogAppService implements IBlogAppService {
    
    getBlogCategories = async (): Promise<GenResponse<BlogCategory[]>> => {
        let objResp: GenResponse<BlogCategory[]> = new GenResponse();
        try{
            objResp = await agent.requests.get<GenResponse<BlogCategory[]>>(apiRoutes.blog.getBlogCategories);
        }catch(error){
            appServices.commonService.LogActivity(`[${`[BlogAppService][getBlogCategories]`}] ${JSON.stringify(error, null, 2)}`);
        }

        return objResp;
    }

    getBlogPosts = async (page?: number, pageCount?: number): Promise<GenResponse<BlogPost[]>> => {
        let objResp: GenResponse<BlogPost[]> = new GenResponse();
        const pageLimit = 50;
        if (!page) page = 0;
        if (!pageCount || (pageCount < 5) || (pageCount > pageLimit)) pageCount = pageLimit;
        try{
            objResp = await agent.requests.get<GenResponse<BlogPost[]>>(apiRoutes.blog.getBlogPosts(page, pageCount));
        }catch(error){
            appServices.commonService.LogActivity(`[${`[BlogAppService][getBlogPosts]`}] ${JSON.stringify(error, null, 2)}`);
            objResp.error = `Error: ${error}`;
            objResp.isSuccess = false;
            objResp.result = [];
            objResp.statCode = StatusCode.ServerError;
        }
        return objResp;
    }
    
    addNewPost = async (newPost : BlogpostNewmodel): Promise<GenResponse<boolean>> => {
        let objResp: GenResponse<boolean> = new GenResponse();
        if (!newPost) {objResp.result= false; objResp.error ="Invalid post content"; return objResp;};
        const initCheck = this.InitCheckIsValidPost(newPost);
        if(initCheck.isSuccess === false){
            return initCheck;
        }
        try{
            objResp = await agent.requests.post<GenResponse<boolean>>(apiRoutes.blog.addNewPost, newPost);
        }catch(error){
            appServices.commonService.LogActivity(`[${`[BlogAppService][addNewPost]`}] ${JSON.stringify(error, null, 2)}`);
            objResp.error = `Error add new post: ${error}`;
            objResp.isSuccess = false;
            objResp.result = false;
            objResp.statCode = StatusCode.ServerError;
        }
        return objResp;
    }

    getSingleBlogPostBySlug = async (slugUrl: string): Promise<GenResponse<BlogPost|null>> => {
        let objResp: GenResponse<BlogPost> = new GenResponse();
        if(!slugUrl){
            objResp.error = 'Invalid post url.';
            objResp.isSuccess = false;
            objResp.result = null;
            objResp.statCode = StatusCode.BadRequest;
            return objResp;
        }
        try{
            objResp = await agent.requests.get<GenResponse<BlogPost>>(apiRoutes.blog.getSingleBlogPostBySlug(slugUrl));
        }catch(error){
            appServices.commonService.LogActivity(`[${`[BlogAppService][getSingleBlogPostBySlug]`}] ${JSON.stringify(error, null, 2)}`);
            objResp.error = `Error: ${error}`;
            objResp.isSuccess = false;
            objResp.result = null;
            objResp.statCode = StatusCode.ServerError;
        }
        return objResp;
    }

    uploadImage = async (): Promise<GenResponse<string>> => {
        let objResp: GenResponse<string> = new GenResponse();
        try{
            objResp = await agent.requests.get(apiRoutes.blog.getBlogCategories);
            
        }catch(error){
            appServices.commonService.LogActivity(`[${`[BlogAppService][uploadImage]`}] ${JSON.stringify(error, null, 2)}`);
        }

        return objResp;
    }

    //#region HELPERS 
    InitCheckIsValidPost = (post: BlogpostNewmodel): GenResponse<boolean> => {
        const objResp: GenResponse<boolean> = new GenResponse();
        const titleMinLength = appsettings.Blog.Posts.Validations.titleMinCharactersLength;
        const contentMinLength = appsettings.Blog.Posts.Validations.contentMinCharactersLength;
        if (!post.title || !post.content || post.title.length < titleMinLength || post.content.length < contentMinLength) {
            objResp.statCode = StatusCode.BadRequest;
            objResp.result = false;
            objResp.isSuccess = false;
            objResp.message = objResp.error = `Invalid post data title or content. Post title must be at least ${titleMinLength} characters and content must be at least ${contentMinLength} characters`;
            return objResp;
        }
        objResp.result = true;
        objResp.statCode = StatusCode.OK;
        objResp.isSuccess = true;
        return objResp;
    }
    //#endregion
}

export interface IBlogAppService {
    getBlogCategories: () => Promise<GenResponse<BlogCategory[]>>;
    getBlogPosts: (page?: number, pageCount?: number)=> Promise<GenResponse<BlogPost[]>>;
}