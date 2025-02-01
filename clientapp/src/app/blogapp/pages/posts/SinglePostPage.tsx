import React, { useEffect, useState } from "react";
import RouteTo from "../../../_components/RouteTo";
import postImg from "../../../../assets/images/postImg.jpeg";
import facebook from "../../../../assets/images/facebook.svg";
import instagram from "../../../../assets/images/instagram.svg";
import userImg from "../../../../assets/images/userImg.jpeg";
import { formatDistanceToNow } from 'date-fns';
import Search from "../../components/Search";
import './posts.css';
import { appServices, useAppStore } from "../../../common/services/appservices";
import { BlogCategory } from "../../models/blog_categories";
import PostMenuActions from "../../components/PostMenuActions";
import Comments from "../../components/Comments";
import { BlogPost } from "../../models/blog_post";
import { useLocation } from "react-router-dom";

const SinglePostPage: React.FC = () => {
  const [categories, setCategories] = useState<BlogCategory[]>([]);
  const location = useLocation();
  const pathname = location.pathname.startsWith('/') ? location.pathname.substring(1) : location.pathname;
  const [post , setPost] = useState<BlogPost>();
  console.log(JSON.stringify(post,null,2));

  const {blogService} = useAppStore();

    useEffect(()=>{
      blogService.getBlogCategories().then((resp)=>{
        if(resp.isSuccess && resp.result){
          setCategories(resp.result);
        }
      }).catch((error)=>{
        appServices.commonService.LogError(JSON.stringify(error,null,2));
      });
    },[blogService]);

    React.useEffect(() => {
      blogService.getSingleBlogPostBySlug(pathname).then((resp)=>{
        if(resp.isSuccess && resp.result){
          setPost(resp.result);
        }
      }).catch((error)=>{
        appServices.commonService.LogError(JSON.stringify(error,null,2));
      });
    },[blogService, pathname]);
  return (
    <>
      <div className="flex flex-col gap-8 mt-4">
        {/* detail */}
        <div className="flex gap-8">
          <div className="lg:w-3/5 flex flex-col gap-8">
            <h1 className="text-xl md:text-3xl xl:text-4xl 2xl:text-5xl font-semibold">
              {post?.title} 
            </h1>
            <div className="flex items-center gap-2 text-gray-400 text-sm">
              <span>Written by</span>
              <RouteTo to={``} className="text-blue-800">{post?.user.name}</RouteTo>
              <span>on</span>
              <RouteTo to={``} className="text-blue-800">{post?.category.name}</RouteTo>
              <span>{formatDistanceToNow(post?.created_at ?? Date.now(), {addSuffix: true})}</span>
              </div>
              <p className="text-gray-500 font-medium">{(post?.description ?? '').length < 10 ? post?.content.slice(0,150) : post?.description}</p>
            </div>
          <div className="hidden lg:block w-2/5">
            <img src={(post?.img && post.img.length > 4) ?post.img: postImg} width={600} className="rounded-2xl max-h-[350px] aspect-video"/>
          </div>
        </div>
        {/* content */}
        <div className="flex flex-col md:flex-row gap-12 justify-between">
        {/* text */}
        <div className="lg:text-lg flex flex-col gap-6 text-justify">
          <div dangerouslySetInnerHTML={{ __html: post?.content ?? '' }} />
           {/* Comments */}
          <Comments postId={`45`} comment={{_id:`45`, createdAt:(new Date(2024, 11,12)).toDateString(), desc:`lorem on web design`, user:{img: userImg, username: 'onadebi'}}} />
        </div>
        {/* menu */}
        <div className="px-4 h-max stickyPoint">
          <h1 className="mb-4 text-sm font-medium">Author</h1>
          <div className="flex flex-col gap-4">
            <div className="flex items-center gap-8">
              {/* {data.user.img && ( */}
                <img
                  src={userImg}
                  className="w-12 h-12 rounded-full object-cover"
                  width="48"
                  height="48"
                />
              {/* )} */}
              <RouteTo to={``} className="text-blue-800">{`Onax`}</RouteTo>
            </div>
            <p className="text-sm text-gray-500">
              Lorem ipsum dolor sit amet consectetur
            </p>
            <div className="flex gap-2">
              <RouteTo to={``}>
                <img src={facebook} />
              </RouteTo>
              <RouteTo to={``}>
                <img src={instagram} />
              </RouteTo>
            </div>
          </div>
          <PostMenuActions post={{}}/>
          <h1 className="mt-8 mb-4 text-sm font-medium">Categories</h1>
          <div className="flex flex-col gap-2 text-sm">
            <RouteTo to={``} className="underline">All</RouteTo>
            {categories.map((cat) => (
              <RouteTo key={cat.id} to={`/posts${cat.url}`} className="underline">
                {cat.categoryName}
              </RouteTo>
            ))}
          </div>
          <h1 className="mt-8 mb-4 text-sm font-medium">Search</h1>
          <Search />
        </div>
      </div>
     
    </div>
    </>
  );
};

export default SinglePostPage;
