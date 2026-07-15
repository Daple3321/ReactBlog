import { useState } from 'react';
import './App.css';
import Blogs from './components/Blogs';
import NavBar from './components/NavBar.jsx'
import BlogView from './components/BlogView.jsx'
import NewBlogForm from './components/NewBlogForm.jsx'
import EditBlogForm from './components/EditBlogForm.jsx'; 
import { removeBlog } from './blogTools.jsx';
import { useAuth } from "react-oidc-context";


export default function App() {
    const auth = useAuth();
    const [pageId, setPage] = useState(0);
    const [currentBlog, setCurrentBlog] = useState();

    if (auth.isLoading) {
        return <div>Loading holding patterns...</div>;
    }

    if (auth.hasError) {
        return <div>Oops! {auth.error.message}</div>;
    }

    if (!auth.isAuthenticated) {
        return (
            <div>
                <button onClick={() => auth.signinRedirect()}>Log in</button>
                <button onClick={() => auth.signinRedirect({ extraQueryParams: { kc_action: 'register' } })}>
                    Register
                </button>
            </div>
        );
    }

    const token = auth.user.access_token;
    
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
        await removeBlog(blog.id, token);
        setCurrentBlog(undefined);
        setPage(0);
    }

    let page;
    if (pageId == 0) {
        page = <Blogs token={token} onBlogClicked={onBlogClicked} />;
    }
    else if (pageId == 1){
        page = <NewBlogForm token={token} onCreated={onBlogCreated} />;
    }
    else if (pageId == 2) {
        page = <BlogView blog={currentBlog} onEditClick={onEditClicked} onRemoveClick={onRemoveClicked} />;
    }
    else if (pageId == 3) {
        page = <EditBlogForm token={token} blogToEdit={currentBlog} onUpdated={onBlogUpdated} />;
    }

    return (
        <div>
            <h1>Welcome, {auth.user.profile.preferred_username ?? auth.user.profile.name}!</h1>
            <button onClick={() => auth.signoutRedirect()}>Log out</button>
            <h2>My Blogs</h2>
            <NavBar onBlogsClick={onBlogsClick} onNewBlogClick={onNewBlogClick} />
            {page}
        </div>
    );
}
