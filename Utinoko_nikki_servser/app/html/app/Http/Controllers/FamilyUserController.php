<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Http\JsonResponse;
use Illuminate\Support\Facades\DB;

class FamilyUserController extends Controller
{
    // 家族ユーザー取得
    public function index(Request $request): JsonResponse
    {
        $id = $request->query('id');

        if ($id !== null && $id !== '') {
            $user = DB::table('family_users')
                ->select('id', 'name', 'firebase_uid')
                ->where('id', $id)
                ->first();

            return response()->json($user);
        }

        $user = DB::table('family_users')
            ->select('id', 'name', 'firebase_uid')
            ->get();

        return response()->json($user);
    }

    // 家族ユーザー登録
    public function store(Request $request): JsonResponse
    {
        // バリデーション
        $validated = $request->validate([
            'name' => 'required|string|max:255',
            'firebase_uid' => 'required|string|max:128|unique:family_users,firebase_uid',
        ]);

        // データ登録
        $id = DB::table('family_users')->insertGetId([
            'name' => $validated['name'],
            'firebase_uid' => $validated['firebase_uid'],
            'created_at' => now(),
            'updated_at' => now(),
        ]);

        // 登録したユーザーを取得
        $user = DB::table('family_users')
            ->select('id', 'name', 'firebase_uid')
            ->where('id', $id)
            ->first();

        return response()->json($user, 201);
    }

    //編集
    public function update(Request $request, int $id): JsonResponse
    {
        // 対象のユーザーが存在するか確認
        $user = DB::table('family_users')
            ->where('id', $id)
            ->first();

        if (!$user) {
            return response()->json([
                'message' => '指定されたユーザーが存在しません。'
            ], 404);
        }

        // バリデーション
        $validated = $request->validate([
            'name' => 'required|string|max:100',
        ]);

        try {
            // ユーザー情報を更新
            DB::table('family_users')
                ->where('id', $id)
                ->update([
                    'name' => $validated['name'],
                    'updated_at' => now(),
                ]);

            // 更新後のユーザー情報を取得
            $user = DB::table('family_users')
                ->select('id', 'name', 'firebase_uid')
                ->where('id', $id)
                ->first();

            return response()->json($user, 200);

        } catch (\Illuminate\Database\QueryException $e) {
            return response()->json([
                'message' => 'ユーザー情報の更新中にデータベースエラーが発生しました。'
            ], 500);
        }
    }

    ###削除
    public function destroy(int $id): JsonResponse
    {
        // 対象のユーザーが存在するか確認
        $user = DB::table('family_users')
            ->where('id', $id)
            ->first();

        if (!$user) {
            return response()->json([
                'message' => '指定されたユーザーが存在しません。'
            ], 404);
        }

        try {
            // ユーザーを削除
            DB::table('family_users')
                ->where('id', $id)
                ->delete();

            return response()->json([
                'message' => 'ユーザーを削除しました。'
            ], 200);

        } catch (\Illuminate\Database\QueryException $e) {

            // 外部キー制約違反
            if (($e->errorInfo[1] ?? null) === 1451) {
                return response()->json([
                    'message' => 'このユーザーは他のデータから参照されているため削除できません。'
                ], 409);
            }

            // その他のデータベースエラー
            return response()->json([
                'message' => 'データベースエラーが発生しました。'
            ], 500);
        }
    }
}
