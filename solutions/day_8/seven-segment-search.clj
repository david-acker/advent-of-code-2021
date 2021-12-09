(ns seven-segment-search
  (:require [clojure.string :as string]))

(def file-name "C:\\Users\\david\\source\\repos\\advent-of-code-2021\\input\\day_8.txt")

(def input-data (string/split-lines (slurp file-name)))

(defn extract-display-values [i]
  (-> i
      (string/split #"\|")
      (nth 1)
      (string/trim)
      (string/split #" ")))

(defn get-all-display-values [x]
  (->> x
       (map extract-display-values)
       (flatten)))

(defn get-part-one-result [input]
  (->> input
       (get-all-display-values)
       (map count)
       (map (fn [i] (.contains [2, 3, 4, 7] i)))
       (filter true?)
       (count)))

;; Part One
(def part-one-result (get-part-one-result input-data))
(println part-one-result)