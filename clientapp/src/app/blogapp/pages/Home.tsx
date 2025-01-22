import React from "react";
import BlogAppRoutes from "../BlogAppRoutes";
import MetaTag from "../../_components/MetaTag";
import RouteTo from "../../_components/RouteTo";
import MainCategories from "../components/MainCategories";
import FeaturedPost from "../components/FeaturedPost";
import PostList from "../components/PostList";
import appsettings from "../../common/config/appsettings";

const Home: React.FC = () => {
  return (
    <>
      <MetaTag title={"Home"} />
      <div>
        <div className="flex gap-4">
          {/*<RouteTo to={BlogAppRoutes().dashboard.home.parentRoute}>
            Dashboard
          </RouteTo>
          <RouteTo to={BlogAppRoutes().public.home.parentRoute}>
            Home
          </RouteTo>
           <span>•</span><span className="text-blue-800">Blogs and Articles</span> */}
        </div>
        <div className="flex items-center justify-between">
          <div className="">
            <h1 className="text-gray-800 text-2xl md:text-5xl lg:text-6xl font-bold"> Crafting Digtal Media Solutions</h1>
            <p className="mt-8 text-md md:text-xl italic text-gray-500">Committed to providing the performant software solutions.</p>
          </div>
          <RouteTo to={BlogAppRoutes().dashboard.write.parentRoute} className="hidden md:block relative">
            <svg viewBox="0 0 200 200" width={200} height={200} className="text-lg tracking-widest animate-spin" style={{animationDuration: '10s'}}>
              <path id='circlePath' d="M 100, 100 m -75, 0 a 75,75 0 1,1 150,0 a 75,75 0 1,1 -150,0" fillOpacity={'8%'} fill="#6c6c"/>
              <text>
                <textPath href="#circlePath" startOffset={`12%`} >{appsettings.appName}</textPath>
                <textPath href="#circlePath" startOffset={`50%`}>Digital Media</textPath>
              </text>
            </svg>
            <button className="absolute top-0 left-0 right-0 bottom-0 m-auto w-20 h-20 bg-blue-800 rounded-full flex items-center justify-center">
            <svg
              xmlns="http://www.w3.org/2000/svg"
              viewBox="0 0 24 24"
              width="50"
              height="50"
              fill="none"
              stroke="white"
              strokeWidth="2"
            >
              <line x1="6" y1="18" x2="18" y2="6" />
              <polyline points="9 6 18 6 18 15" />
            </svg>
          </button>
          </RouteTo>
        </div>

        <MainCategories/>
        <FeaturedPost/>
        <div className="">
          <h1 className="my-8 text-2xl text-gray-600">
            Recent Posts
          </h1>
          <PostList/>
        </div>
      </div>
    </>
  );
};

export default Home;
