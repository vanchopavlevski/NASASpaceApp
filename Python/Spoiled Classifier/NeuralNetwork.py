
# coding: utf-8

# In[1]:

import tensorflow as tf
import numpy as np


# In[2]:

class NeuralNetwork:
    def __init__(self,learningRate):
        self.X=None
        self.Y=None
        self.Y_Output=None
        self.current_input=None
        self.cost=None
        self.learningRate=learningRate
        self.optimizer=None
        self.Sess=None
        self.objSaver = None
        
    def AddInputLayer(self,imageWidth,imageHeight,outputClasses):
        self.X = tf.placeholder(tf.float32,shape=[None,imageWidth,imageHeight,3],name='X')
        self.Y = tf.placeholder(tf.float32,shape=[None,outputClasses],name='Y')
        self.current_input = self.X

    def AddConvolutionLayer(self,layerName,filterSize,outputChannels,strides):
        with tf.variable_scope("Encoder/ConvolutionLayer/{0}".format(layerName)):
            n_input = self.current_input.get_shape().as_list()[3]

            W = tf.get_variable(name='W',shape=[filterSize,filterSize,n_input,outputChannels],
                                initializer=tf.random_normal_initializer(mean=0.0,stddev=0.01))
            B = tf.get_variable(name='B',shape=[outputChannels],
                                initializer=tf.constant_initializer(0.01))
            h = tf.nn.conv2d(self.current_input,W,strides=[1,strides,strides,1],padding='VALID')
            
            self.current_input = tf.nn.relu(tf.nn.bias_add(h,B))
    
    def AddFlattenLayer(self,layerName):
        with tf.variable_scope("Encoder/FlattenLayer/{0}".format(layerName)):
            shapeList = self.current_input.get_shape().as_list()
            finalShape = shapeList[1] * shapeList[2] * shapeList[3]
            self.current_input = tf.reshape(self.current_input,shape=[-1,finalShape])
    
    def AddDenseLayer(self,layerName,nInputNeurons,nOutputNeurons,activationFunction):
        with tf.variable_scope(layerName):
            W1 = tf.get_variable(name='W_dense',shape=[nInputNeurons,nOutputNeurons],
                            initializer=tf.random_normal_initializer(mean=0.0,stddev=0.01))
            b1 = tf.get_variable('B_dense',shape=[nOutputNeurons],initializer=tf.constant_initializer(0.01))
            h1 = tf.nn.bias_add(tf.matmul(self.current_input,W1),b1)
            
            self.current_input = activationFunction(h1)
    
    def MSE(self):
        self.Y_Output=self.current_input
        self.cost = tf.reduce_mean(tf.reduce_mean(tf.squared_difference(self.Y,self.Y_Output),1))
    
    def CrossEntropy(self):
        self.Y_Output=self.current_input
        #log_out = tf.log(self.Y_Output)
        #if tf.is
        
        self.cost = tf.reduce_mean(-tf.reduce_sum(self.Y * tf.log(tf.clip_by_value(self.Y_Output,1e-10,1.0)), reduction_indices=[1]))
        #self.cost = tf.reduce_mean(-tf.reduce_sum(self.Y * tf.log(self.Y_Output), reduction_indices=[1]))
        
    def Compile(self,optimizerName):
        if optimizerName=='Adam':
            self.optimizer = tf.train.AdamOptimizer(learning_rate=self.learningRate).minimize(self.cost)
        else:
            raise ValueError('Optimizer function is not found!')
        self.Sess = tf.Session()
        self.Sess.run(tf.global_variables_initializer())
        
    def Train(self,input_data,output_data):
        self.Sess.run(self.optimizer,feed_dict={self.X:input_data,self.Y:output_data})
        error = self.Sess.run(self.cost,feed_dict={self.X:input_data,self.Y:output_data})
        return error
    
    def SaveModel(self,saveFilePath):
        objSaver = tf.train.Saver()
        save_path = objSaver.save(self.Sess, saveFilePath)
        print("Model saved in file: %s" % save_path)
    
    def LoadModel(self,modelFilePath):
        if self.Sess==None:
            self.Sess = tf.Session()
            self.Sess.run(tf.global_variables_initializer())
        objSaver = tf.train.Saver()
        objSaver.restore(self.Sess, modelFilePath)

    def Predict(self,input_data,roundVector=True):
        result = self.Sess.run(self.Y_Output,feed_dict={self.X:input_data})
        if roundVector:
            return np.rint(result)
        else:
            return result


# objNeuralNetwork = NeuralNetwork(0.01)
# 
# objNeuralNetwork.AddInputLayer(200,200,3)
# 
# objNeuralNetwork.AddConvolutionLayer('Layer1',5,5,2)
# 
# objNeuralNetwork.AddFlattenLayer('Layer2')
# 
# objNeuralNetwork.AddDenseLayer('Layer3',50000,128,tf.nn.relu)
# 
# objNeuralNetwork.AddDenseLayer('Layer4',128,64,tf.nn.relu)
# 
# objNeuralNetwork.AddDenseLayer('Layer5',64,3,tf.nn.relu)
# 
# objNeuralNetwork.MSE()
# 
# objNeuralNetwork.Compile('Adam')

# In[ ]:



