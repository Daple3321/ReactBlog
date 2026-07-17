function BlogView({ blog, canEdit, onEditClick, onRemoveClick }) {
    if (blog != null) {
        const createdDate = new Date(blog.createdAt).toLocaleString();
        const updatedDate = new Date(blog.lastUpdatedAt).toLocaleString();
        return (
            <div>
                <h1 id="blogTitle">{blog.name}</h1>
                
                <ul style={{color: '#74439ad9'} }>
                    <li id="createdAt" style={{ display: 'inline-block', padding: '14px 16px' }}>
                        Created at: {createdDate}
                    </li>

                    <li id="updatedAt" style={{ display: 'inline-block', padding: '14px 16px' }}>
                        Updated at: {updatedDate}
                    </li>
                </ul>
                
                <article id="blogContent">{blog.content}</article>
                
                {canEdit && (
                    <ul>
                        <li id="editBlog" style={{ display: 'inline-block', padding: '14px 16px' }}>
                            <button onClick={() => onEditClick(blog)}>Edit</button>
                        </li>
                        <li id="removeBlog" style={{ display: 'inline-block', padding: '14px 16px' }}>
                            <button onClick={()=>onRemoveClick(blog)}>Remove</button>
                        </li>
                    </ul>
                )}
            </div>
        );
    }
    else {
        return (
            <div>
                <h1 id="blogTitle">Blog title</h1>
                <div>
                    <p id="createdAt">
                        Created at:
                    </p>
                    <p id="updatedAt">
                        Updated at:
                    </p>
                </div>
                <p id="blogContent">Content</p>
            </div>
        );
    }
}

export default BlogView;