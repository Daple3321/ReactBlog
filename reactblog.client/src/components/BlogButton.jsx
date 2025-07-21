import { useEffect, useState } from 'react';

export default function BlogButton({ b, onBlogClicked }) {
    const [blog, setBlog] = useState(b);

    function handleClick() {
        console.log("Clicked on blog with id: " + blog.id)
    }
    
    return (
        <button id="blogBtn" onClick={()=> onBlogClicked(blog)}>
            <h1>{blog.name}</h1>
            <p>Created at: {blog.createdAt}</p>
        </button>
    );
}
