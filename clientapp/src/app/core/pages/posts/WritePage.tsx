import "react-quill-new/dist/quill.snow.css";
import ReactQuill from "react-quill-new";
import { FormEvent, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { toast } from "react-toastify";
import Upload from "../../components/Upload";
import { appServices, useAppStore } from "../../../common/services/appservices";
import { BlogCategory } from "../../models/blog_categories";
import { BlogpostNewmodel } from "../../models/BlogPostNewModel";
import { useMutation } from "@tanstack/react-query";

const WritePage = () => {
  const [postdetail, setPostDetail] = useState<BlogpostNewmodel>({
    img:"",
    title: "",
    category: "1",
    description: "",
    content: "",
    is_featured: false,
    is_active: true,
  });
  // const [value, setValue] = useState("");
  // const [cover, setCover] = useState("");
  const [postImg, setPostImg] = useState("");
  const [videoInPost, setVideoInPost] = useState("");
  const [progress, setProgress] = useState(0);

  const [blogCategories, setBlogCategories] = useState<BlogCategory[]>([]);

  const {blogService} = useAppStore();

  useState(async () => {
    const categories = await blogService.getBlogCategories();
    if(categories.isSuccess && categories.result){
      setBlogCategories(categories.result);
    }
  });

  useEffect(() => {
    if (postImg) {
      setPostDetail((prev) => ({
        ...prev,
        content: prev.content + `<p style="display:flex; align-items: center; justify-content:center;"><img width="500px" src="${postImg}" style="height:400px; aspect-ratio: 16/9; max-width: 80%; max-height: 50%;"/></p>`,
      }));
    }
  }, [postImg]);
    
  useEffect(() => {
    if(videoInPost){
      setPostDetail((prev) => ({
        ...prev,
        content: prev.content + `<p><iframe class="ql-video" src="${videoInPost}"/></p>`
      }));
    }
  }, [videoInPost]);

  const navigate = useNavigate();

  const mutation = useMutation({
    mutationFn: async (newPost: BlogpostNewmodel) => {
      const objResp =  await appServices.blogService.addNewPost(newPost);
      return objResp;
    },
    onSuccess: (res) => {
      if(res.isSuccess){
        toast.success("Post created successfully");
        console.log(res.message);
        navigate(`/${res.message}`);
      }else{
        console.log(res.error);
        toast.error(res.error);
        alert(`/${res.error}`);
      }
    },
  });

  const handleChange = (evt: FormEvent<HTMLInputElement|HTMLSelectElement|HTMLTextAreaElement>) => {
    evt.preventDefault();
    const { name, value } = evt.currentTarget;
    setPostDetail({ ...postdetail, [name]: value });
  };

  const handleChangeEditor = (value: string| ReactQuill.UnprivilegedEditor) => {
    setPostDetail({ ...postdetail, content: value.toString() });
  }
  
  const handleSetCover =(cover: string)=> {
    // setCover(cover);
    setPostDetail({ ...postdetail, img: cover });
  }

  const handleSubmit = async (evt: FormEvent)=> {
    evt.preventDefault();
    mutation.mutate(postdetail);
  }

  return (
    <div className="h-[calc(100vh-64px)] md:h-[calc(100vh-80px)] flex flex-col gap-2">
      <h1 className="text-cl font-light">Create a New Post</h1>
      <form onSubmit={handleSubmit} className="flex flex-col gap-6 flex-1 mb-6">
        <Upload fileType="image" setProgress={setProgress} setData={handleSetCover}>
          <button type="button" className="w-max p-2 shadow-md rounded-xl text-sm text-gray-500 bg-white">
            Add a cover image
          </button>
        </Upload>
        {(postdetail.img && postdetail.img.length > 4) &&(<img src={postdetail.img} width={350} height={350} className="w-full max-h-[650px] max-w-[650px] aspect-video"/>)}
        <input className="text-4xl font-semibold bg-transparent outline-none" onChange={handleChange} value={postdetail.title} type="text" placeholder="Post title" name="title" />
        <div className="flex gap-4 flex-col items-start md:flex-row md:items-center lg:flex-row lg:items-center">
          <label htmlFor="category" className="text-sm select-none">
            Choose a category:
          </label>
          <select name="category" id="category" className="p-2 rounded-xl bg-white shadow-md" onChange={handleChange}>
            { blogCategories ?
              blogCategories.sort((a,b)=> a.order - b.order).map((cat) => (
                <option key={cat.id} value={cat.id}>{cat.categoryName}</option>
              ))
              : <option value="loading">Loading...</option>
            }
          </select>
          <div className="flex items-center gap-2">
            <label htmlFor="is_active" className="select-none" title="publish immediately">Publish Immediately</label>
            <input type="checkbox" name="is_active" id="is_active" value={postdetail.is_featured.toString()} onChange={(e) =>setPostDetail({...postdetail, is_active: e.target.checked})} />
          </div>
          <div className="flex items-center gap-2">
            <label htmlFor="is_featured" className="select-none" title="Set as featured">Set as featured</label>
            <input type="checkbox" name="is_featured" id="is_featured" value="is_featured" onChange={(e) =>setPostDetail({...postdetail, is_featured: e.target.checked})} />
          </div>
        </div>
        <textarea className="p-4 rounded-xl bg-white shadow-md md:w-full lg:w-full" onChange={handleChange} value={postdetail.description} name="description" placeholder="A Short Description"/>
        <div className="flex flex-1 ">
          <div className="flex flex-col gap-2 mr-2">
            <Upload fileType="image" setProgress={setProgress} setData={setPostImg}>🌆</Upload>
            <Upload fileType="video" setProgress={setProgress} setData={setVideoInPost}>▶️</Upload>
          </div>
          <ReactQuill theme="snow"
            className="flex-1 rounded-xl bg-white shadow-md min-h-[300px]"
            value={postdetail.content} onChange={handleChangeEditor} readOnly={0 < progress && progress < 100}
          />
        </div>
        <button disabled={mutation.isPending}
          className="bg-blue-800 text-white font-medium rounded-xl mt-4 p-2 w-36 disabled:bg-blue-400 disabled:cursor-not-allowed">
          {mutation.isPending ? "Loading..." : "Send"}
        </button>
        {"Progress:" + progress}
        {mutation.isError && <span>{mutation.error.message}</span>}
      </form>
    </div>
  );
};

export default WritePage;