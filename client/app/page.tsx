'use client';

import { useEffect, useState } from 'react';
import { CreatePostForm } from '@/components/create-post-form';
import { PostCard } from '@/components/post-card';
import { RestoreDbCard } from '@/components/restore-db-card';
import { getAllPosts, Post } from '@/lib/api';

export default function Home() {
  const [posts, setPosts] = useState<Post[]>([]);
  const [loading, setLoading] = useState(true);

  const fetchPosts = async () => {
    try {
      const data = await getAllPosts();
      setPosts(data);
    } catch (error) {
      console.error('Failed to fetch posts:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchPosts();
  }, []);

  return (
    <div className="container mx-auto py-8 space-y-8 max-w-4xl">
      <h1 className="text-4xl font-bold">Social Media Posts</h1>
      
      <RestoreDbCard onRestore={fetchPosts} />
      
      <CreatePostForm onPostCreated={fetchPosts} />

      {loading ? (
        <p className="text-center text-muted-foreground">Loading posts...</p>
      ) : posts.length === 0 ? (
        <p className="text-center text-muted-foreground">No posts yet. Create the first one!</p>
      ) : (
        <div className="space-y-4">
          {posts.map((post) => (
            <PostCard key={post.postId} post={post} onUpdate={fetchPosts} />
          ))}
        </div>
      )}
    </div>
  );
}
