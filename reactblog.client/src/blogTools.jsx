import React, { createContext, useState, useContext } from 'react';
import App from './App.jsx'

const PageContext = createContext();
const CurrentBlogContext = createContext();

export function PageProvider({ children }) {
      const [pageId, setPage] = useState(0);

      const setCurrentPage = n => {
        setPage(n);
      };

      return (
        <PageContext.Provider value={{ pageId, setCurrentPage }}>
          {children}
        </PageContext.Provider>
      );
}
export function usePage() {
      return useContext(PageContext);
}

export function BlogProvider({ children }) {
      const [currentBlog, setCurrentBlog] = useState();
      const setBlog = b => {
        setCurrentBlog(b);
      };

      return (
        <CurrentBlogContext.Provider value={{ currentBlog, setCurrentBlog }}>
          {children}
        </CurrentBlogContext.Provider>
      );
}
export function useBlog() {
      return useContext(CurrentBlogContext);
}



export async function getBlog(blogId) {
    const response = await fetch(`/blogs/${blogId}`, {
            method: "GET",
        });
    return await response.json();
}

export async function removeBlog(blogId) {
    const response = await fetch(`/blogs/${blogId}`, {
            method: "DELETE",
        });
}

export function switchPageToBlog(blog){
    const {setCurrentPage} = usePage();
    const {setCurrentBlog} = useBlog();
    setCurrentPage(blog.blogId);
    setCurrentBlog(blog);
}