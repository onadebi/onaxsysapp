import React from 'react';
import PostListItem from './PostListItem';
import { useQuery } from '@tanstack/react-query';
import { appServices } from '../../common/services/appservices';
import { BlogPost } from '../models/blog_post';
import { useDispatch } from 'react-redux';
import { hideLoader, setLoading } from '../../../store/slices/common/Common.slice';

const PostList: React.FC = () => {
  const dispatch = useDispatch();
  const { data, isLoading, isFetched } = useQuery<BlogPost[]>({
    queryKey: ['blogposts/fetchall'],
    queryFn: async () => {
      const res = await appServices.blogService.getBlogPosts();
      return res.data as BlogPost[];
    },
  });

  if (isLoading) {dispatch(setLoading({display:true,  message:'Loading posts...'})); return null;}
  if (isFetched) {dispatch(hideLoader());}

  return (
    <div className='flex flex-col gap-12 mb-8'>
        {
          data ?
            data.sort((a,b)=> new Date(b.created_at).getTime() - new Date(a.created_at).getTime()).map((post,index)=>(
            <PostListItem key={index} author={post.user} category={post.category} 
            content={post.content} description={post.description} date={post.created_at} imagePath={post.img} title={post.title} slugUrl={post.slug} />
            ))
            : <p>No posts found.</p>
        }
    </div>
  )
}

export default PostList;