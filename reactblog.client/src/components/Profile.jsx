import { useEffect, useState } from 'react';
import BlogButton from './BlogButton.jsx';
import {
    followUser,
    getFollowers,
    getFollowing,
    getUser,
    getUserBlogs,
    unfollowUser,
} from '../blogTools.jsx';

export default function Profile({
    username,
    currentUserId,
    currentUsername,
    token,
    onBlogClicked,
    onUserClicked,
}) {
    const [profile, setProfile] = useState(null);
    const [blogs, setBlogs] = useState(null);
    const [followers, setFollowers] = useState(null);
    const [following, setFollowing] = useState(null);
    const [error, setError] = useState(null);
    const [followBusy, setFollowBusy] = useState(false);

    useEffect(() => {
        let cancelled = false;
        setProfile(null);
        setBlogs(null);
        setFollowers(null);
        setFollowing(null);
        setError(null);

        Promise.all([
            getUser(username, token),
            getUserBlogs(username, token),
            getFollowers(username, token),
            getFollowing(username, token),
        ])
            .then(([user, blogResult, followerResult, followingResult]) => {
                if (cancelled) return;
                setProfile(user);
                setBlogs(blogResult.items);
                setFollowers(followerResult);
                setFollowing(followingResult);
            })
            .catch(e => {
                if (!cancelled) setError(e.message);
            });

        return () => { cancelled = true; };
    }, [token, username]);

    if (error) return <p className="error-message">Failed to load profile: {error}</p>;
    if (profile === null || blogs === null || followers === null || following === null) {
        return <p>Loading profile...</p>;
    }

    const isOwnProfile = profile.id === currentUserId;
    const isFollowing = followers.some(user => user.id === currentUserId);

    async function toggleFollow() {
        setFollowBusy(true);
        setError(null);

        try {
            if (isFollowing) {
                await unfollowUser(profile.username, token);
                setFollowers(users => users.filter(user => user.id !== currentUserId));
            } else {
                await followUser(profile.username, token);
                setFollowers(users =>
                    [...users, { id: currentUserId, username: currentUsername }]
                        .sort((a, b) => a.username.localeCompare(b.username))
                );
            }
        } catch (e) {
            setError(e.message);
        } finally {
            setFollowBusy(false);
        }
    }

    function userLinks(users, emptyText) {
        if (users.length === 0) return <p>{emptyText}</p>;

        return (
            <ul className="user-list">
                {users.map(user => (
                    <li key={user.id}>
                        <button className="user-link" onClick={() => onUserClicked(user.username)}>
                            {user.username}
                        </button>
                    </li>
                ))}
            </ul>
        );
    }

    return (
        <section className="profile">
            <div className="profile-heading">
                <h1>{isOwnProfile ? 'Your blogs' : `${profile.username}'s blogs`}</h1>
                {!isOwnProfile && (
                    <button onClick={toggleFollow} disabled={followBusy}>
                        {followBusy ? 'Saving...' : isFollowing ? 'Unfollow' : 'Follow'}
                    </button>
                )}
            </div>

            {blogs.length === 0
                ? <p>No blogs yet.</p>
                : (
                    <ul className="blogs-list">
                        {blogs.map(blog => (
                            <li key={blog.id}>
                                <BlogButton
                                    b={blog}
                                    onBlogClicked={() => onBlogClicked(blog, isOwnProfile)}
                                />
                            </li>
                        ))}
                    </ul>
                )}

            <div className="profile-connections">
                <section>
                    <h2>Followers ({followers.length})</h2>
                    {userLinks(followers, 'No followers yet.')}
                </section>
                <section>
                    <h2>Following ({following.length})</h2>
                    {userLinks(following, 'Not following anyone yet.')}
                </section>
            </div>
        </section>
    );
}
