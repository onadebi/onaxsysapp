import React from 'react';
import featured1 from '../../../assets/images/featured1.jpeg';
import featured2 from '../../../assets/images/featured2.jpeg';
import featured3 from '../../../assets/images/featured3.jpeg';
import RouteTo from '../../_components/RouteTo';

const FeaturedPost: React.FC = () => {
const featuredPosts = [{
    id: 1,
    title: 'Lorem ipsum dolor sit amet consectetur adipisicing elit.',
    category: 'Software',
    date: '2 days ago',
    image: featured1
  },
  {
    id: 2,
    title: 'Lorem ipsum dolor sit amet consectetur adipisicing elit.',
    category: 'Development',
    date: '2 days ago',
    image: featured2
},
{
    id: 3,
    title: 'Lorem ipsum dolor sit amet consectetur adipisicing elit.',
    category: 'Marketing',
    date: '2 days ago',
    image: featured3
}]
  return (
    <div className='mt-8 flex flex-col lg:flex-row gap-8'>
        
        <div className="w-full lg:w-1/2 flex flex-col gap-4">
            <img src={featured1} alt="featured post" className="rounded-3xl object-cover"/>
            <div className="flex items-center gap-2">
                <h1 className='font-semibold lg:text-lg'>01.</h1>
                <RouteTo to={``} className='text-onaxBlue lg:text-lg'>Software</RouteTo>
                <span className='text-gray-500'>2 days ago</span>
            </div>
        <RouteTo to={`/test`} className='text-xl lg:text-3xl font-semibold lg:font-bold'>
            Lorem ipsum dolor sit amet consectetur adipisicing elit.
        </RouteTo>
        </div>
        <div className="w-full lg:w-1/2 flex flex-col gap-4">
        {
            featuredPosts.map((post,index) => (
                <div className="lg:h-1/3 flex justify-between gap-4" key={index}>
                    <img src={post.image} alt="featured post" className="rounded-3xl object-cover w-1/3 aspect-video"/>
                    <div className='w-2/3'>
                        <div className="flex items-center gap-4 text-sm lg:text-base mb-4">
                            <h1 className='font-semibold'>0{index+1}.</h1>
                            <RouteTo to={`#`} className='text-onaxBlue lg:text-lg'>{post.category}</RouteTo>
                            <span className='text-gray-500'>{post.date}</span>
                        </div>
                        <RouteTo to={`/test`} className='sm:text-lg md:text-2xl lg:text-xl xl:text-2xl font-medium'>{post.title}</RouteTo>
                    </div>
                </div>
            ))
        }
        </div>
    </div>
  )
}

export default FeaturedPost;