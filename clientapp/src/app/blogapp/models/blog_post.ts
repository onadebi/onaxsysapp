export type BlogPost = {
    post_id: string
    user_id: string
    img: string
    title: string
    slug: string
    description: string
    category: Category
    content: string
    is_featured: boolean
    is_active: boolean
    is_deleted: boolean
    visit: number
    created_at: string
    updated_at: string
    user: User
}

export interface Category {
    name: string
    category_slug: string
}

export interface User {
    name: string
    user_id: string
}
