import { createBrowserRouter } from "react-router-dom";
import GeneralLayout from "../app/blogapp/layouts/GeneralLayout";
import Home from "../app/blogapp/pages/Home";
import BlogAppRoutes from "../app/blogapp/BlogAppRoutes";
// import DashboardLayout from "../app/blogapp/layouts/DashboardLayout";
import Dashboard from "../app/blogapp/pages/dashboard/Dashboard";
import RegisterPage from "../app/blogapp/pages/register/RegisterPage";
import LoginPage from "../app/blogapp/pages/auth/LoginPage";
import WritePage from "../app/blogapp/pages/posts/WritePage";
// import SinglePostPage from "../app/blogapp/pages/posts/SinglePostPage";
// import PostListPage from "../app/blogapp/pages/posts/PostListPage";
import UploadTest from "../app/blogapp/pages/register/UploadTest";
import DashboardBaseLayout from "../app/blogapp/layouts/DashboardBaseLayout";

const AppRouter = createBrowserRouter([
    {
        path: '/',
        element: <GeneralLayout/>,
        children: [
            {
                path: BlogAppRoutes().public.home.parentRoute,
                element: <Home/>
            },
            {
                path: BlogAppRoutes().public.register.parentRoute,
                element: <RegisterPage/>
            },
            {
                path: BlogAppRoutes().public.login.parentRoute,
                element: <LoginPage/>
            },
            // {
            //     path: BlogAppRoutes().public.postDetail.parentRoute,
            //     element: <SinglePostPage/>
            // },
            // {
            //     path: BlogAppRoutes().public.posts.parentRoute,
            //     element: <PostListPage/>
            // },
        ]        
    },
    {
        path: '/dashboard',
        element: <DashboardBaseLayout/>,
        children :[
            {
                path: BlogAppRoutes().dashboard.home.parentRoute,
                element: <Dashboard/>
            },
            {
                path: BlogAppRoutes().dashboard.write.parentRoute,
                element: <WritePage/>
            },
            {
                path: BlogAppRoutes().dashboard.test.parentRoute,
                element: <UploadTest/>
            },
        ]
    },
    {
        path: '*',
        element: <>Not FOund</>
    },
]
,{
    basename:'/zone/portal'
}
);

export default AppRouter;