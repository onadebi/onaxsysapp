import { createBrowserRouter } from "react-router-dom";
import GeneralLayout from "../app/core/layouts/GeneralLayout";
import Home from "../app/core/pages/Home";
import BlogAppRoutes from "../app/core/BlogAppRoutes";
// import DashboardLayout from "../app/core/layouts/DashboardLayout";
import Dashboard from "../app/core/pages/dashboard/Dashboard";
import RegisterPage from "../app/core/pages/register/RegisterPage";
import LoginPage from "../app/core/pages/auth/LoginPage";
import WritePage from "../app/core/pages/posts/WritePage";
// import SinglePostPage from "../app/core/pages/posts/SinglePostPage";
// import PostListPage from "../app/core/pages/posts/PostListPage";
import UploadTest from "../app/core/pages/register/UploadTest";
import DashboardBaseLayout from "../app/core/layouts/DashboardBaseLayout";

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