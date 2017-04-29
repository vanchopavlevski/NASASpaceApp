
# coding: utf-8

# In[29]:

import numpy as np
import tensorflow as tf
import pandas
import matplotlib.pyplot as plt
import h5py
from skimage.transform import resize
from sys import stdout

import matplotlib.pyplot as plt


# In[3]:

class DataSet:
    def __init__(self,DataSetFilePathCSV,OutputFile,imageWidth,imageHeight):
        self.dataSetFilePath = DataSetFilePathCSV
        self.width = imageWidth
        self.height= imageHeight
        self.rows=None
        self.OutputFile = OutputFile
        self.maxClass = None
        
        self.DataFrame = pandas.read_csv(self.dataSetFilePath)
        self.maxClass = self.DataFrame["Class"].max()
        self.rows = self.DataFrame.shape[0]
        
        print(self.rows,self.maxClass)
    
    def CreateHDF5DataSet(self):
        with h5py.File(self.OutputFile,'w') as h:
            Features = h.create_dataset('Features', (self.rows,self.width,self.height,3),maxshape=(None, self.width,self.height,3), dtype='f')
            Labels   = h.create_dataset('Labels', (self.rows,int(self.maxClass)),maxshape=(None, int(self.maxClass)), dtype='f')

            count=0
            percentCount=0
            totalFiles = self.rows
            for row in self.DataFrame.iterrows():
                filePath = row[1][0]
                productClass = row[1][1]

                percentDone = (100.0 * (count+1)) / totalFiles

                stdout.write("\rProcessing: %s of %s - %s%% - %s" % ((count+1),totalFiles,round(percentDone,2),filePath))
                stdout.flush()

                image = plt.imread(filePath)/255
                where_are_NaNs = np.isnan(image)
                image[where_are_NaNs] = 0
                image = resize(image,(self.width,self.height),mode='reflect')

                Features[count] = np.array(image)
                Labels[count] = self.GetLabel(int(productClass))
                print("Labela: {0}".format(self.GetLabel(int(productClass))))
                count+=1
    
    def GetLabel(self,intNumberClass):
        result_array = [0] * self.maxClass
        result_array[intNumberClass-1]=1
        #result_array = np.reshape(result_array,[1,self.maxClass])
        return result_array
    
    def GetSample(self,sampleIndex):
        with h5py.File(self.OutputFile,'r') as h:
            Features = h.get('Features')
            Labels = h.get('Labels')
            return np.array(Features[sampleIndex]),np.array(Labels[sampleIndex])

    def GenerateTrainingData(self,batchSize):
        with h5py.File(self.OutputFile,'r') as h:
            Features = h.get('Features')
            Labels = h.get('Labels')
            
            rand_idx = np.random.permutation(self.rows)
            number_of_batches = self.rows//batchSize
            for batch_i in range(number_of_batches):
                idxs_i = rand_idx[batch_i * batchSize: (batch_i + 1) * batchSize]
                
                Features_List = []
                Labels_List = []
                
                for element in idxs_i:
                    Features_List.append(Features[element])
                    Labels_List.append(Labels[element])
                    #plt.imshow(Features[element])
                    #plt.show()
                    #print(Labels[element])
                    
                yield np.array(Features_List),np.array(Labels_List)


# objDataSet = DataSet("DataSet.csv",200,200)
# objDataSet.CreateHDF5DataSet("TrainingSet.hdf5")

# objDataSet = DataSet('DataSet.csv',200,200)
# objDataSet.CreateHDF5DataSet('Test.hdf5')

# In[ ]:



