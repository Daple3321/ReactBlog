import { createContext, useState, useContext } from 'react';

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



export function authorizedFetch(url, token, options = {}) {
    return fetch(url, {
        ...options,
        headers: { ...options.headers, Authorization: `Bearer ${token}` },
    });
}

export async function ensureMe(token) {
    const response = await authorizedFetch('/me', token, { method: 'POST' });
    if (!response.ok) {
        throw new Error(`Failed to ensure user (${response.status})`);
    }
    return response.json();
}

export async function getBlog(blogId, token) {
    const response = await authorizedFetch(`/blogs/${blogId}`, token, {
            method: "GET",
        });
    return await response.json();
}

export async function removeBlog(blogId, token) {
    return authorizedFetch(`/blogs/${blogId}`, token, {
            method: "DELETE",
        });
}

async function getJson(url, token) {
    const response = await authorizedFetch(url, token);
    if (!response.ok) throw new Error(`Request failed (${response.status})`);
    return response.json();
}

export function getUsers(token) {
    return getJson('/users', token);
}

export function getUser(username, token) {
    return getJson(`/users/${encodeURIComponent(username)}`, token);
}

export function getUserBlogs(username, token) {
    return getJson(`/users/${encodeURIComponent(username)}/blogs`, token);
}

export function getFollowers(username, token) {
    return getJson(`/users/${encodeURIComponent(username)}/followers`, token);
}

export function getFollowing(username, token) {
    return getJson(`/users/${encodeURIComponent(username)}/following`, token);
}

export async function followUser(username, token) {
    const response = await authorizedFetch(`/users/${encodeURIComponent(username)}/follow`, token, {
        method: 'POST',
    });
    if (!response.ok) throw new Error(`Could not follow user (${response.status})`);
}

export async function unfollowUser(username, token) {
    const response = await authorizedFetch(`/users/${encodeURIComponent(username)}/follow`, token, {
        method: 'DELETE',
    });
    if (!response.ok) throw new Error(`Could not unfollow user (${response.status})`);
}
