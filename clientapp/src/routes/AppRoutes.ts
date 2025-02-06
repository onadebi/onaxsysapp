const AppRoutes = () => {
    return {
        homebase: "/zone/portal",
        public: {
            home: {
                name: "Home",
                parentRoute: "/"    
            },
            register: {
                name: "Register",
                parentRoute: '/register',
            },
            login: {
                name: "Login",
                parentRoute: '/login',
            },
            posts: {
                name: "Posts",
                parentRoute: '/posts',
            },
            postDetail: {
                name: "Post Detail",
                parentRoute: '/:postSlug',
            },
        },
        dashboard:{
            home:{
                name: "Dashboard",
                parentRoute: '/dashboard'
            },
            write: {
                name: "Write Post",
                parentRoute: '/dashboard/write',
            },
            test: {
                name: "Write Post",
                parentRoute: '/dashboard/testwrite',
            },
        }
    }
};

export default AppRoutes;