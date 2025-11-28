'use client';

import { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Post, likePost, deletePost, editPost, addComment, editComment, removeComment } from '@/lib/api';

interface PostCardProps {
  post: Post;
  onUpdate: () => void;
}

export function PostCard({ post, onUpdate }: PostCardProps) {
  const [isEditing, setIsEditing] = useState(false);
  const [editedMessage, setEditedMessage] = useState(post.message);
  const [isAddingComment, setIsAddingComment] = useState(false);
  const [commentUsername, setCommentUsername] = useState('');
  const [commentText, setCommentText] = useState('');
  const [editingCommentId, setEditingCommentId] = useState<string | null>(null);
  const [editedComment, setEditedComment] = useState('');
  const [editedCommentUsername, setEditedCommentUsername] = useState('');

  const handleLike = async () => {
    try {
      await likePost(post.postId);
      setTimeout(onUpdate, 500); // Wait for eventual consistency
    } catch (error) {
      console.error('Failed to like post:', error);
      alert('Failed to like post');
    }
  };

  const handleDelete = async () => {
    if (!confirm('Are you sure you want to delete this post?')) return;
    try {
      await deletePost(post.postId);
      setTimeout(onUpdate, 500);
    } catch (error) {
      console.error('Failed to delete post:', error);
      alert('Failed to delete post');
    }
  };

  const handleEdit = async () => {
    try {
      await editPost(post.postId, { message: editedMessage });
      setIsEditing(false);
      setTimeout(onUpdate, 500);
    } catch (error) {
      console.error('Failed to edit post:', error);
      alert('Failed to edit post');
    }
  };

  const handleAddComment = async () => {
    try {
      await addComment(post.postId, { username: commentUsername, comment: commentText });
      setCommentUsername('');
      setCommentText('');
      setIsAddingComment(false);
      setTimeout(onUpdate, 500);
    } catch (error) {
      console.error('Failed to add comment:', error);
      alert('Failed to add comment');
    }
  };

  const handleEditComment = async (commentId: string) => {
    try {
      await editComment(post.postId, commentId, { username: editedCommentUsername, comment: editedComment });
      setEditingCommentId(null);
      setTimeout(onUpdate, 500);
    } catch (error) {
      console.error('Failed to edit comment:', error);
      alert('Failed to edit comment');
    }
  };

  const handleRemoveComment = async (commentId: string, username: string) => {
    if (!confirm('Are you sure you want to remove this comment?')) return;
    try {
      await removeComment(post.postId, commentId, username);
      setTimeout(onUpdate, 500);
    } catch (error) {
      console.error('Failed to remove comment:', error);
      alert('Failed to remove comment');
    }
  };

  return (
    <Card>
      <CardHeader>
        <CardTitle className="flex justify-between items-center">
          <span>{post.author}</span>
          <span className="text-sm text-muted-foreground">
            {new Date(post.datePosted).toLocaleDateString()}
          </span>
        </CardTitle>
      </CardHeader>
      <CardContent className="space-y-4">
        {isEditing ? (
          <div className="space-y-2">
            <Textarea
              value={editedMessage}
              onChange={(e) => setEditedMessage(e.target.value)}
              rows={4}
            />
            <div className="flex gap-2">
              <Button onClick={handleEdit}>Save</Button>
              <Button variant="outline" onClick={() => setIsEditing(false)}>Cancel</Button>
            </div>
          </div>
        ) : (
          <p className="text-sm">{post.message}</p>
        )}

        <div className="flex gap-2">
          <Button onClick={handleLike} variant="outline" size="sm">
            👍 Like ({post.likes})
          </Button>
          <Button onClick={() => setIsEditing(!isEditing)} variant="outline" size="sm">
            Edit
          </Button>
          <Button onClick={handleDelete} variant="destructive" size="sm">
            Delete
          </Button>
          <Button onClick={() => setIsAddingComment(!isAddingComment)} variant="outline" size="sm">
            💬 Comment ({post.comments.length})
          </Button>
        </div>

        {isAddingComment && (
          <div className="space-y-2 p-4 border rounded">
            <Input
              placeholder="Your username"
              value={commentUsername}
              onChange={(e) => setCommentUsername(e.target.value)}
            />
            <Textarea
              placeholder="Your comment"
              value={commentText}
              onChange={(e) => setCommentText(e.target.value)}
              rows={2}
            />
            <div className="flex gap-2">
              <Button onClick={handleAddComment} size="sm">Add Comment</Button>
              <Button variant="outline" onClick={() => setIsAddingComment(false)} size="sm">Cancel</Button>
            </div>
          </div>
        )}

        {post.comments.length > 0 && (
          <div className="space-y-2">
            <h4 className="font-semibold text-sm">Comments:</h4>
            {post.comments.map((comment) => (
              <div key={comment.commentId} className="p-3 bg-muted rounded text-sm">
                {editingCommentId === comment.commentId ? (
                  <div className="space-y-2">
                    <Input
                      placeholder="Username"
                      value={editedCommentUsername}
                      onChange={(e) => setEditedCommentUsername(e.target.value)}
                    />
                    <Textarea
                      value={editedComment}
                      onChange={(e) => setEditedComment(e.target.value)}
                      rows={2}
                    />
                    <div className="flex gap-2">
                      <Button onClick={() => handleEditComment(comment.commentId)} size="sm">Save</Button>
                      <Button variant="outline" onClick={() => setEditingCommentId(null)} size="sm">Cancel</Button>
                    </div>
                  </div>
                ) : (
                  <>
                    <div className="flex justify-between items-start">
                      <div>
                        <span className="font-semibold">{comment.username}</span>
                        {comment.edited && <span className="text-xs text-muted-foreground ml-2">(edited)</span>}
                        <p className="mt-1">{comment.comment}</p>
                      </div>
                      <span className="text-xs text-muted-foreground">
                        {new Date(comment.commentDate).toLocaleDateString()}
                      </span>
                    </div>
                    <div className="flex gap-2 mt-2">
                      <Button
                        onClick={() => {
                          setEditingCommentId(comment.commentId);
                          setEditedComment(comment.comment);
                          setEditedCommentUsername(comment.username);
                        }}
                        variant="outline"
                        size="sm"
                      >
                        Edit
                      </Button>
                      <Button
                        onClick={() => handleRemoveComment(comment.commentId, comment.username)}
                        variant="destructive"
                        size="sm"
                      >
                        Remove
                      </Button>
                    </div>
                  </>
                )}
              </div>
            ))}
          </div>
        )}
      </CardContent>
    </Card>
  );
}
