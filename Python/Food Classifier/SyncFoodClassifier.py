
# coding: utf-8

# In[2]:

import tensorflow as tf
import numpy as np
import NeuralNetwork
import matplotlib.pyplot as plt
from skimage.transform import resize
import sys
import os
os.environ['TF_CPP_MIN_LOG_LEVEL'] = '3'


# In[3]:

objSyncFoodClassifier = None


# In[2]:

class SyncFoodClassifier:
    def __init__(self):
        self.objNeuralNetwork = NeuralNetwork.NeuralNetwork(0.0001)
        self.objNeuralNetwork.AddInputLayer(100,100,6)
        self.objNeuralNetwork.AddConvolutionLayer('ConvolutionLayer1',5,32,2)
        self.objNeuralNetwork.AddConvolutionLayer('ConvolutionLayer2',5,16,2)
        self.objNeuralNetwork.AddConvolutionLayer('ConvolutionLayer3',5,16,2)
        self.objNeuralNetwork.AddConvolutionLayer('ConvolutionLayer4',3,8,2)
        #objNeuralNetwork.AddConvolutionLayer('ConvolutionLayer3',5,4,2)
        self.objNeuralNetwork.AddFlattenLayer('FlattenLayer3')
        #objNeuralNetwork.AddDenseLayer('DenseLayer1',2048,1024,tf.nn.relu)
        #self.objNeuralNetwork.AddDenseLayer('DenseLayer2',648,512,tf.nn.relu)
        #self.objNeuralNetwork.AddDenseLayer('DenseLayer3',512,128,tf.nn.relu)
        self.objNeuralNetwork.AddDenseLayer('DenseLayer4',128,32,tf.nn.relu)
        self.objNeuralNetwork.AddDropOut()
        self.objNeuralNetwork.AddDenseLayer('DenseLayer5',32,6,tf.nn.softmax)
        self.objNeuralNetwork.CrossEntropy()
        self.objNeuralNetwork.Compile('Adam')
        self.objNeuralNetwork.LoadModel("tmp/model.ckpt")
    
    def GetClass(self,imageFilePath,classFilePath):
        image = plt.imread(imageFilePath)/255
        where_are_NaNs = np.isnan(image)
        image[where_are_NaNs] = 0
        image = resize(image,(100,100),mode='reflect')
        image = np.expand_dims(image,axis=0)
        result = self.objNeuralNetwork.Predict(image,roundVector=False)
        maxIndex = np.argmax(result)
        maxIndex = maxIndex

        with open(classFilePath) as f:
            content = f.readlines()
            content = [x.strip() for x in content] 
        return content[maxIndex]


# In[5]:

if objSyncFoodClassifier==None:
    objSyncFoodClassifier = SyncFoodClassifier()


# In[5]:

test_input = []
for i in range(1,len(sys.argv)):
    test_input.append(sys.argv[i])

filePath = ' '.join(test_input)


# In[ ]:

#print(filePath)


# In[5]:

result = objSyncFoodClassifier.GetClass(filePath,'Classes.txt')


# In[6]:

print("Python: {0}".format(result))


# In[ ]:



