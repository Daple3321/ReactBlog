import { useEffect, useState } from 'react';
import './App.css';
import Blogs from './components/Blogs';
import NavBar from './components/NavBar.jsx'
import BlogButton from './components/BlogButton.jsx'
import BlogView from './components/BlogView.jsx'
import NewBlogForm from './components/NewBlogForm.jsx'
import EditBlogForm from './components/EditBlogForm.jsx'; 
import { removeBlog } from './blogTools.jsx';


export default function App() {
    const [pageId, setPage] = useState(0);
    const [currentBlog, setCurrentBlog] = useState();
    
    function onBlogsClick() {
        setPage(0);
    }
    function onNewBlogClick() {
        setPage(1);
    }
    function onBlogCreated(newBlog) {
        setCurrentBlog(newBlog);
        setPage(2);
    }
    function onBlogClicked(blog) {
        setCurrentBlog(blog);
        setPage(2);
    }
    
    function onEditClicked(editBlog){
        setCurrentBlog(editBlog);
        setPage(3);
    }
    function onBlogUpdated(updatedBlog) {
        setCurrentBlog(updatedBlog);
        setPage(2);
    }
    
    async function onRemoveClicked(blog){
        await removeBlog(blog.id);
        setCurrentBlog(undefined);
        setPage(0);
    }

    if (pageId == 0) {
        return (
            <div>
                <NavBar onBlogsClick={onBlogsClick} onNewBlogClick={onNewBlogClick} />
                <Blogs onBlogClicked={(blog) => onBlogClicked(blog)} />
            </div>
        );
    }
    else if (pageId == 1){
        return (
            <div>
                <NavBar onBlogsClick={onBlogsClick} onNewBlogClick={onNewBlogClick} />
                <NewBlogForm onCreated={b=>onBlogCreated(b)}/>
            </div>
        );
    }
    else if (pageId == 2) {
        return (
            <div>
                <NavBar onBlogsClick={onBlogsClick} onNewBlogClick={onNewBlogClick} />
                <BlogView blog={currentBlog} onEditClick={b=> onEditClicked(b)} onRemoveClick={b=>onRemoveClicked(b)}/>
            </div>
        );
    }
    else if (pageId == 3) {
        return (
            <div>
                <NavBar onBlogsClick={onBlogsClick} onNewBlogClick={onNewBlogClick} />
                <EditBlogForm blogToEdit={currentBlog} onUpdated={b => onBlogUpdated(b)} />
            </div>
        );
    }
    /*return (
        <div>
            <h1 id="tableLabel">Blogs</h1>
            {blogContents}
        </div>
    );*/
}
