import { useEffect, useState } from 'react';
import BlogButton from './BlogButton';

export default function Blogs({ onBlogClicked }) {
    let [blogs, setBlogs] = useState();

    useEffect(() => {
        populateBlogData();
    }, []);

    const blogContents = blogs === undefined
        ? <p>Loading...</p>
        : <div>
            {
                blogs.map(blog =>
                    <li key={blog.id}>
                        <BlogButton b={blog} onBlogClicked={onBlogClicked} />
                    </li>
                )
            }
        </div>

    return (
        <div>
            <ul>{blogContents}</ul>
        </div>
    );

    async function populateBlogData() {
        const response = await fetch('/blogs/all');
        console.log(response)
        if (response.ok) {
            const data = await response.json();
            setBlogs(data);
        }
    }
}
