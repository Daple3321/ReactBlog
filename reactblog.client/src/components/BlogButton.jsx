export default function BlogButton({ b: blog, onBlogClicked }) {
    return (
        <button className="blog-card" onClick={() => onBlogClicked(blog)}>
            <h2>{blog.name}</h2>
            <p className="blog-date">Created: {new Date(blog.createdAt).toLocaleDateString()}</p>
        </button>
    );
}
