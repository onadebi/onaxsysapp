import { useSearchParams } from "react-router-dom";
import Search from "./Search";
import React, { FormEvent, useState, useEffect } from 'react';
import { BlogCategory } from "../models/blog_categories";
import { appServices, useAppStore } from "../../common/services/appservices";

const SideMenu: React.FC = () => {
  const [blogCategories, setBlogCategories] = useState<BlogCategory[]>([]);
  const [searchParams, setSearchParams] = useSearchParams();

  const {blogService} = useAppStore();
  
  useEffect(() => {
    blogService.getBlogCategories().then((resp)=>{
      if(resp.isSuccess && resp.result){
        setBlogCategories(resp.result);
      }
    }).catch((error)=>{
      appServices.commonService.LogError(JSON.stringify(error,null,2));
    });
  },[blogService]);

  const handleFilterChange = (e: FormEvent<HTMLInputElement>) => {
    if (searchParams.get("sort") !== e.currentTarget.value) {
      setSearchParams({
        ...Object.fromEntries(searchParams.entries()),
        sort: e.currentTarget.value,
      });
    }
  };
  const handleCategoryChange = (category: string) => {
    if (searchParams.get("cat") !== category) {
      setSearchParams({
        ...Object.fromEntries(searchParams.entries()),
        cat:category,
      });
    }
  };

  return (
    <div className="px-4 h-max sticky top-8">
      <h1 className="mb-4 text-sm font-medium">Search</h1>
      <Search />
      <h1 className="mt-8 mb-4 text-sm font-medium">Filter</h1>
      <div className="flex flex-col gap-2 text-sm">
        <label htmlFor="" className="flex items-center gap-2 cursor-pointer">
          <input type="radio" name="sort" onChange={handleFilterChange} value="newest"
            className="appearance-none w-4 h-4 border-[1.5px] border-blue-800 cursor-pointer rounded-sm bg-white checked:bg-blue-800"/>
          Newest
        </label>
        <label htmlFor="" className="flex items-center gap-2 cursor-pointer">
          <input type="radio" name="sort" onChange={handleFilterChange} value="popular"
            className="appearance-none w-4 h-4 border-[1.5px] border-blue-800 cursor-pointer rounded-sm bg-white checked:bg-blue-800"/>
          Most Popular
        </label>
        <label htmlFor="" className="flex items-center gap-2 cursor-pointer">
          <input type="radio" name="sort" onChange={handleFilterChange} value="trending"
            className="appearance-none w-4 h-4 border-[1.5px] border-blue-800 cursor-pointer rounded-sm bg-white checked:bg-blue-800"/>
          Trending
        </label>
        <label htmlFor="" className="flex items-center gap-2 cursor-pointer">
          <input type="radio" name="sort" onChange={handleFilterChange} value="oldest"
            className="appearance-none w-4 h-4 border-[1.5px] border-blue-800 cursor-pointer rounded-sm bg-white checked:bg-blue-800"/>
          Oldest
        </label>
      </div>
      <h1 className="mt-8 mb-4 text-sm font-medium">Categories</h1>
      <div className="flex flex-col gap-2 text-sm">
        <span className="underline cursor-pointer" onClick={()=>handleCategoryChange("general")}>All</span>
        {
            blogCategories?.sort((a,b)=> a.order -b.order).map((cat) => (
                <span key={cat.id} className="underline cursor-pointer" onClick={()=>handleCategoryChange(cat.slug)}>{cat.categoryName}</span>
            ))
        }
      </div>
    </div>
  )
}

export default SideMenu;