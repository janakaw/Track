import tensorflow as tf
import  matplotlib.pyplot as plt

from tensorflow.examples.tutorials.mnist import input_data

X = tf.placeholder(tf.float32, shape=[None, 784])
W = tf.Variable(tf.zeros([784, 10]))
b = tf.Variable(tf.zeros([10]))
init = tf.global_variables_initializer()

#model
Y = tf.nn.softmax(tf.matmul(X, W) + b)

# placholder for correct answers
Y_ = tf.placeholder(tf.float32, [None, 10])

# loss function
cross_entropy = -tf.reduce_sum(Y_ * tf.log(Y))

# % of correct answer found in batch
is_correct = tf.equal(tf.argmax(Y,1), tf.argmax(Y_,1))
accuracy = tf.reduce_mean(tf.cast(is_correct, tf.float32))
optimizer = tf.train.GradientDescentOptimizer(0.003)
train_step = optimizer.minimize(cross_entropy)

sess = tf.Session()
sess.run(init)

mnist = input_data.read_data_sets('MNIST_data', one_hot=True)
acc = []
ce = []
x = []

for i in range(1000):
	# load batch of images and correct answers
	batch_X, batch_Y = mnist.train.next_batch(100)
	train_data={X: batch_X, Y_:batch_Y}

	# train 
	sess.run(train_step, train_data)

	a, c = sess.run([accuracy, cross_entropy], train_data)
	x.append(i)
	acc.append(a)
	ce.append(c)

	test_data = {X:mnist.test.images, Y_:mnist.test.labels}
	a,c = sess.run([accuracy, cross_entropy], test_data)

plt.plot(acc)
plt.ylabel('accuracy')
plt.show()

plt.plot(ce)
plt.ylabel('cross entropy')
plt.show()



