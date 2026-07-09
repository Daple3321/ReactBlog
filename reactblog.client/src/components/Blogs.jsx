import { useEffect, useState } from 'react';
import BlogButton from './BlogButton';

export default function Blogs({ onBlogClicked }) {
    const [blogs, setBlogs] = useState(null);
    const [error, setError] = useState(null);

    useEffect(() => {
        fetch('/blogs')
            .then(r => r.ok ? r.json() : Promise.reject(r.status))
            .then(setBlogs)
            .catch(e => setError(`Failed to load blogs (${e})`));
    }, []);

    const blogContents = error
        ? <p style={{ color: 'salmon' }}>{error}</p>
        : blogs === null
            ? <p>Loading...</p>
            : blogs.length === 0
                ? <p>No blogs yet.</p>
                : blogs.map(blog =>
                    <li key={blog.id}>
                        <BlogButton b={blog} onBlogClicked={onBlogClicked} />
                    </li>
                );

    return (
        <ul className="blogs-list">{blogContents}</ul>
    );
}
