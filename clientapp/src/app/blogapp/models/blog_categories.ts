export type BlogCategory =  {
    id: number,
    categoryName: string,
    url: string,
    slug: string,
    description?: string,
    order: number,
    is_active: boolean,
    is_displayed: boolean,
    classname_style?: string,
    icon?: string
  }