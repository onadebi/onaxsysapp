import React from 'react';
import featured2 from '../../../assets/images/featured2.jpeg';
import featured3 from '../../../assets/images/featured3.jpeg';
import featured4 from '../../../assets/images/featured4.jpeg';
import RouteTo from '../../_components/RouteTo';
import { formatDistanceToNow } from 'date-fns';

export interface IProps{
    imagePath: string;
    title: string;
    description: string;
    category: {name: string, category_slug: string};
    date: string;
    content: string;
    author: {name: string, user_id: string};
    slugUrl: string
}

const PostListItem: React.FC<IProps> = ({author,category,content,date,description,imagePath,title, slugUrl}) => {

  const images = [featured2, featured3, featured4];
  const randomSelection = images[Math.floor(Math.random() * images.length)];

  return (
    <div className='flex flex-col xl:flex-row gap-8'>
        <div className="md:hidden xl:block xl:w-1/4">
            <img src={(imagePath && imagePath.length > 4) ?  imagePath: randomSelection} alt="image post" loading='lazy' width={500} height={600} className="rounded-2xl object-cover aspect-auto"/>
        </div>
        <div className="flex flex-col gap-4 xl:w-3/4">
            <RouteTo to={`/${slugUrl}`} className='text-4xl font-semibold'>
            {title}
            </RouteTo>
            <div className="flex items-center gap-2 text-gray-400 text-sm">
                <span>Written by</span>
                <RouteTo to={author.user_id} className='text-onaxBlue'>{author.name}</RouteTo>
                <span>on</span>
                <RouteTo to={category.category_slug} className='text-onaxBlue'>{category.name}</RouteTo>
                <RouteTo to={`#`} className='text-onaxBlue'>{` ${formatDistanceToNow(date,{addSuffix:true})}`}</RouteTo>
            </div>
            <p>
            {(description && description.length > 150) ? <div dangerouslySetInnerHTML={{ __html: description.slice(0,450) ?? '' }} /> : <div dangerouslySetInnerHTML={{ __html: content.slice(0, 450) ?? '' }} /> }
            </p>
            <RouteTo to={`/${slugUrl}`} className='underline text-blue-800 text-sm'>Read more</RouteTo>
        </div>
    </div>
  )
}

export default PostListItem;