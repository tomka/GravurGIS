#ifndef QUADTREE_H
#define QUADTREE_H

typedef struct s_bbox2d {
  double	minX;
  double	maxY;
  double	width;
  double	height;
} *bbox2d;

typedef struct s_quadtree_node {
  bbox2d box;
  struct s_quadtree_node *childs[4];
  void *data;
} *quadtree_node;

/*top left x , top left y, bottom right x, bottom right y, subdivision level*/
/*Number of leaf:4^level*/
extern quadtree_node quadtree_create(float tlx, float tly, float brx, float bry, unsigned int level);
/*Do node->data=f(node,data) for each leaf*/
extern void quadtree_foreach_leaf(quadtree_node node, void *data, void (*f)(quadtree_node , void *));
#endif