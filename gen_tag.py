import os
import sys
import random
print("{")
randoms = []
for i in range(0,64):
    val = random.randint(0,256)
    randoms.append(str(val))
print(", ".join(randoms))
print("}")