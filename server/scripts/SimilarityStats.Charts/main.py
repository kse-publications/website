import json
import matplotlib.pyplot as plt
import seaborn as sns
import numpy as np


base_path = '../SimilarityStats.DataProvider/data/'


def plot_knn_similarity_scores(file_1, file_2):
    with open(file_1, 'r') as ukrainian_file:
        data1 = json.load(ukrainian_file)
        distances1 = [item['Distance'] for item in data1]

    with open(file_2, 'r') as all_file:
        data2 = json.load(all_file)
        distances2 = [item['Distance'] for item in data2]

    sns.histplot(distances1, bins=30, kde=True, color='blue', label='Lang=Ukrainian', stat='density')

    sns.histplot(distances2, bins=30, kde=True, color='red', label='All', stat='density')

    plt.title('Distribution of Publication Similarities')
    plt.xlabel('Distance')
    plt.ylabel('Density')
    plt.legend(title='Query')
    plt.show()


def get_average_counts(data):
    thresholds = sorted([float(th) for th in data[0]['NeighboursPerDistance'].keys()])
    average_counts = {th: [] for th in thresholds}

    for publication in data:
        for th, count in publication['NeighboursPerDistance'].items():
            average_counts[float(th)].append(count)

    average_counts = {th: np.mean(counts) for th, counts in average_counts.items()}
    return average_counts


def plot_vector_range_similarity_distances(file_paths):
    for file_path in file_paths:
        with open(file_path, 'r') as file:
            data = json.load(file)
        average_counts = get_average_counts(data)
        thresholds = list(average_counts.keys())
        counts = list(average_counts.values())
        plt.plot(thresholds, counts, marker='o', label=file_path.split('/')[-1])

    plt.title('Average Number of Similar Publications per Threshold below 0.6')
    plt.xlabel('Threshold')
    plt.ylabel('Average Count')
    plt.grid(True)
    plt.legend()
    plt.show()


def plot_specific_threshold(file_path, threshold):
    with open(file_path, 'r') as file:
        data = json.load(file)

    count = sum(1 for item in data if item['NeighboursPerDistance'][f'{threshold}'] > 2)
    print(f"Number of publications with more than 2 neighbours at threshold {threshold}: {count}")

    ids = [item['Id'] for item in data]
    neighbours = [item['NeighboursPerDistance'][f'{threshold}'] for item in data]
    colors = ['b' if item['Language'] != 'Ukrainian' else 'r' for item in data]

    plt.bar(ids, neighbours, color=colors)
    plt.title(f'Number of Neighbours at Threshold {threshold} per Publication ID')
    plt.xlabel('Publication ID')
    plt.ylabel(f'Number of Neighbours at Threshold {threshold}')
    plt.show()


def get_average_counts_v2(data):
    thresholds = sorted([float(th) for th in data[0]['NeighboursPerDistance'].keys()])
    languages = set([item['Language'] for item in data])
    average_counts = {lang: {th: [] for th in thresholds} for lang in languages}

    for publication in data:
        for th, count in publication['NeighboursPerDistance'].items():
            average_counts[publication['Language']][float(th)].append(count)

    average_counts = {lang: {th: np.mean(counts) for th, counts in lang_data.items()} for lang, lang_data in average_counts.items()}
    return average_counts


def plot_vector_range_distances(file_path, threshold=0.6):
    with open(file_path, 'r') as file:
        data = json.load(file)
    average_counts = get_average_counts_v2(data)

    for language, lang_data in average_counts.items():
        thresholds = list(lang_data.keys())
        counts = list(lang_data.values())
        plt.plot(thresholds, counts, marker='o', label=language)

    plt.title(f'Average Number of Similar Publications per Threshold below {threshold}')
    plt.xlabel('Threshold')
    plt.ylabel('Average Count')
    plt.grid(True)
    plt.legend()
    plt.show()


# plot_knn_similarity_scores('data/scores-all-pairs-triple-title.json',
#                            'data/scores-lang-ukrainian-pairs-triple-title.json')
#
# plot_vector_range_similarity_distances(
#     ['data/scores-all-average-per-threshold-below-0.6-triple-title.json',
#      'data/scores-ukrainian-average-per-threshold-below-0.6-triple-title.json',
#         'data/scores-english-average-per-threshold-below-0.6-triple-title.json'])

# plot_specific_threshold('data/scores-all-average-per-threshold-below-0.6-triple-title_2.json', 0.32)

plot_vector_range_distances(base_path + 'scores-average-per-threshold-lt-0.6_new.json', 0.6)
