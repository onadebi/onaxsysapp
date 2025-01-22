const apiRoutes = {
    auth:{
        logout: `/api/auth/logout`,
        login: `/api/auth/login`,
        socialRegisterWithLogin: `/api/auth/register`,
    },
    blog: {
        uploadImage: `/api/blog/posts/upload-auth`,
        getBlogCategories: `/api/blog/categories`,
        getBlogPosts: (page?: number, pageCount?: number)=> `/api/blog/post/fetchallposts?page=${page}&pageCount=${pageCount}`,
        getSingleBlogPostBySlug: (slugUrl: string)=> `/api/blog/post/fetchpostbyslug/${slugUrl}`,
        fetchPublicCourseById: (courseId: string) => `/CourseGen/GetPublicCourseById/${courseId}`,
        addNewPost: `/api/blog/post/newpost`,
    },
}

export default apiRoutes;