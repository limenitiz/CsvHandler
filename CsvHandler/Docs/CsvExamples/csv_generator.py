# example use:
# python3 ./csv_generator.py --metric-min 1000 --metric-max 2000 --shift 365 --count-rows 30 > ./invalid-date.csv

from datetime import datetime, timedelta
import random
import sys


metric_min = 0
metric_max = 0
count_rows = 0
shift = 0.5

for i in range(1, len(sys.argv), 2):
    if sys.argv[i] == "--metric-min":
        metric_min = int(sys.argv[i+1])
    elif sys.argv[i] == "--metric-max":
        metric_max = int(sys.argv[i+1])
    elif sys.argv[i] == "--count-rows":
        count_rows = int(sys.argv[i+1])
    elif sys.argv[i] == "--shift":
        shift = float(sys.argv[i+1])

for i in range(24, count_rows + 24):
    dt = datetime.now() - timedelta(hours=24*i*shift)

    s = f'{dt.strftime("%Y-%m-%d_%I-%M-%S")};' + \
        f'{random.randint(0,1000):0>4d};' + \
        f'{random.uniform(metric_min, metric_max):0>4.3f}'
    
    print(s)

