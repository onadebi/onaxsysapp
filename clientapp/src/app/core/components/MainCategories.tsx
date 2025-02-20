import { Link } from "react-router-dom";
import Search from "./Search";
import { useEffect, useState } from "react";
import { BlogCategory } from "../models/blog_categories";
import { appServices } from "../../common/services/appservices";
import RouteTo from "../../_components/RouteTo";

const MainCategories = () => {
  const [categories, setCategories] = useState<BlogCategory[]>([]);
  useEffect(()=>{
    appServices.blogService.getBlogCategories().then((resp)=>{
      if(resp.isSuccess){
        setCategories(resp.result!);
      }
    }).catch((error)=>{
      appServices.commonService.LogError(JSON.stringify(error,null,2));
    });
  },[]);
  return (
    <div className="hidden md:flex bg-white rounded-3xl xl:rounded-full p-4 shadow-lg items-center justify-center gap-8">
      {/* links */}
      <div className="flex-1 flex items-center justify-between flex-wrap">
        <Link
          to="/posts"
          className="bg-blue-800 text-white rounded-full px-4 py-2"
        >
          All Posts
        </Link>
        {categories.map((cat) => (
          <RouteTo
            key={cat.id}
            to={`/posts${cat.url}`}
            className={cat.classname_style}
          >
            {cat.categoryName}
          </RouteTo>
        ))}
      </div>
      <span className="text-xl font-medium">|</span>
      {/* search */}
        {/* search */}
        <Search/>
    </div>
  );
};

export default MainCategories;